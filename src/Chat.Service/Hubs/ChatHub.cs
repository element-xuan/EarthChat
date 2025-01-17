﻿using Chat.Contracts.Chats;
using Chat.Contracts.Hubs;
using Chat.Service.Application.Chats.Commands;
using Chat.Service.Application.Chats.Queries;
using Chat.Service.Application.Hubs.Commands;
using Chat.Service.Services;
using Masa.Contrib.Authentication.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Service.Hubs;

public class ChatHub : Hub
{
    private readonly IEventBus _eventBus;
    private readonly RedisClient _redisClient;
    private readonly BackgroundTaskService _backgroundTaskService;

    public ChatHub(RedisClient redisClient, IEventBus eventBus, BackgroundTaskService backgroundTaskService)
    {
        _redisClient = redisClient;
        _eventBus = eventBus;
        _backgroundTaskService = backgroundTaskService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId == null || userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException();
        }

        await _redisClient.SetAsync(Constant.OnLineKey + userId.Value.ToString("N"), userId.Value);
        await _redisClient.LPushAsync("Connections:" + userId.Value, Context.ConnectionId);

        var groupsQuery = new GetUserGroupQuery(userId.Value);
        await _eventBus.PublishAsync(groupsQuery);
        foreach (var groupDto in groupsQuery.Result)
        {
            // 加入群组
            await Groups.AddToGroupAsync(Context.ConnectionId, groupDto.Id.ToString("N"));
        }

        var systemCommand = new SystemCommand(new Notification()
        {
            createdTime = DateTime.Now,
            type = NotificationType.GroupUserNew,
            content = "新人用户上线",
            data = userId
        }, groupsQuery.Result.Select(x => x.Id).ToArray(), true);

        await _eventBus.PublishAsync(systemCommand);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // 移除在线用户
        var userId = GetUserId();
        if (userId.HasValue)
        {
            await _redisClient.DelAsync(Constant.OnLineKey + userId.Value.ToString("N"));
            await _redisClient.LRemAsync(Constant.Connections + userId, 0, Context.ConnectionId);

            var groupsQuery = new GetUserGroupQuery(userId.Value);
            await _eventBus.PublishAsync(groupsQuery);

            var systemCommand = new SystemCommand(new Notification()
            {
                createdTime = DateTime.Now,
                type = NotificationType.GroupUserNew,
                content = "用户下线",
            }, groupsQuery.Result.Select(x => x.Id).ToArray(), true);

            await _eventBus.PublishAsync(systemCommand);
        }
    }

    /// <summary>
    /// 群聊消息转发
    /// </summary>
    /// <param name="value"></param>
    /// <param name="groupId"></param>
    /// <param name="type"></param>
    /// <param name="revertId"></param>
    public async Task SendMessage(string value, Guid groupId, int type, Guid? revertId = null)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return;
        }

        var userId = GetUserId();
        // 未登录用户不允许发送消息
        if (userId == null || userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException();
        }

        string key = $"user:{userId}:count";

        // 限制用户发送消息频率
        if (await _redisClient.ExistsAsync(key))
        {
            var count = await _redisClient.GetAsync<int>(key);

            // 限制用户发送消息频率每分钟20条
            if (count > 20) return;
        }

        var message = new ChatMessageDto
        {
            Content = value,
            Type = (ChatType)type,
            UserId = userId.Value,
            CreationTime = DateTime.Now,
            RevertId = revertId,
            GroupId = groupId,
            Id = Guid.NewGuid()
        };

        var createChat = new CreateChatMessageCommand(new CreateChatMessageDto
        {
            Content = value,
            Id = message.Id,
            RevertId = revertId,
            ChatGroupId = groupId,
            Type = (ChatType)type,
            UserId = userId.Value
        });

        // 如果发送的内容关联了回复id则查询回复内容
        if (message.RevertId != null && message.RevertId != Guid.Empty)
        {
            var messageQuery = new GetMessageQuery((Guid)message.RevertId);
            await _eventBus.PublishAsync(messageQuery);
            message.Revert = messageQuery.Result;
        }

        if (await _redisClient.ExistsAsync(key))
        {
            await _redisClient.IncrByAsync(key, 1);
        }
        else
        {
            await _redisClient.IncrByAsync(key, 1);
            await _redisClient.ExpireAsync(key, 60);
        }

        // 发送消息新增事件
        await _eventBus.PublishAsync(createChat);

        // 转发到客户端
        _ = Clients.Groups(groupId.ToString("N")).SendAsync("ReceiveMessage", groupId, message);

        // 发送智能助手订阅事件
        await _backgroundTaskService.WriteAsync(new AssistantDto()
        {
            Id = groupId,
            Value = value,
            Group = true,
            RevertId = message.Id,
            UserId = userId.Value
        });
    }

    /// <summary>
    /// 获取当前用户id
    /// </summary>
    /// <returns></returns>
    public Guid? GetUserId()
    {
        var userId = Context.User?.FindFirst(x => x.Type == ClaimType.DEFAULT_USER_ID);

        if (userId == null) return null;

        if (string.IsNullOrEmpty(userId.Value)) return null;

        return Guid.Parse(userId.Value);
    }
}