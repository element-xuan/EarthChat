﻿using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Domain.Users.Repositories;

public interface IFriendRepository : IBaseRepository<Friend,Guid>
{
    Task<List<ChatGroup>> GetUserInFriendAsync(Guid userId);
}