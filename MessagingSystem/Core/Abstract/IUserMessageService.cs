using Domain.Entities;
using System.Collections.Generic;

namespace Core.Abstract
{
    public interface IUserMessageService
    {
        IList<UserMessage> GetInbox(int userId,bool includeDeleted = false);

        UserMessage GetUserMessage(int messageId,int userId, bool includeDeleted = false);

        void MakeMessageRead(UserMessage message);

        void MoveMessageToTrash(UserMessage message);

        void DeleteUserMessage(UserMessage message);

        IList<UserMessage> GetUserMessagesByMessageId(int messageId);

        IList<UserMessage> GetTrashUserMessagesByUserId(int userId, bool includeDeleted = false);

        void RestoreUserMessageToInbox(UserMessage message);
    }
}
