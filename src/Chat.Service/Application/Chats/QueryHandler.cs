﻿using AutoMapper;
using Chat.Contracts.Chats;
using Chat.Service.Application.Chats.Queries;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Domain.Users.Repositories;

namespace Chat.Service.Application.Chats;

public class QueryHandler
{
    private readonly IMapper _mapper;
    private readonly IFriendRepository _friendRepository;
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IChatGroupRepository _chatGroupRepository;
    private readonly IChatGroupInUserRepository _chatGroupInUserRepository;

    public QueryHandler(IChatMessageRepository chatMessageRepository, IMapper mapper,
        IChatGroupInUserRepository chatGroupInUserRepository, IChatGroupRepository chatGroupRepository,
        IFriendRepository friendRepository)
    {
        _chatMessageRepository = chatMessageRepository;
        _mapper = mapper;
        _chatGroupInUserRepository = chatGroupInUserRepository;
        _chatGroupRepository = chatGroupRepository;
        _friendRepository = friendRepository;
    }

    [EventHandler]
    public async Task GetListAsync(GeChatMessageListQuery query)
    {
        var list = await _chatMessageRepository.GetListAsync(query.groupId, query.page, query.pageSize);

        query.Result = new PaginatedListBase<ChatMessageDto>
        {
            Result = _mapper.Map<List<ChatMessageDto>>(list.OrderBy(x => x.CreationTime))
        };
    }

    [EventHandler]
    public async Task GetUserGroupAsync(GetUserGroupQuery query)
    {
        var groups = await _chatGroupInUserRepository.GetUserChatGroupAsync(query.userId);
        var friends = await _friendRepository.GetUserInFriendAsync(query.userId);

        var chatGroups = new List<ChatGroupDto>(groups.Count + friends.Count);
        chatGroups.AddRange(groups.Select(chatGroup => new ChatGroupDto()
        {
            Avatar = chatGroup.Avatar,
            Id = chatGroup.Id,
            CreationTime = chatGroup.CreationTime,
            Creator = chatGroup.Creator,
            Default = chatGroup.Default,
            Description = chatGroup.Description,
            Name = chatGroup.Name,
            Group = true
        }));

        chatGroups.AddRange(friends.Select(friend => new ChatGroupDto()
        {
            Avatar = friend.Avatar,
            Id = friend.Id,
            CreationTime = friend.CreationTime,
            Creator = friend.Creator,
            Default = friend.Default,
            Description = friend.Description,
            Name = friend.Name,
            Group = false
        }));

        query.Result = _mapper.Map<List<ChatGroupDto>>(chatGroups);
    }

    [EventHandler]
    public async Task GetGroupInUserAsync(GetGroupInUserQuery query)
    {
        var result = await _chatGroupInUserRepository.GetGroupInUserAsync(query.groupId);

        query.Result =
            _mapper.Map<List<UserDto>>(result);

        query.Result.Add(new UserDto()
        {
            Id = Guid.Empty,
            Account = string.Empty,
            Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png",
            Name = "聊天机器人",
        });
    }

    [EventHandler]
    public async Task GetGroupAsync(GetGroupQuery query)
    {
        var value = await _chatGroupRepository.FindAsync(x => x.Id == query.Id);

        query.Result = _mapper.Map<ChatGroupDto>(value);
    }

    [EventHandler]
    public async Task GetMessageAsync(GetMessageQuery query)
    {
        var value = await _chatMessageRepository.FindAsync(x => x.Id == query.Id);

        query.Result = _mapper.Map<ChatMessageDto>(value);
    }
}