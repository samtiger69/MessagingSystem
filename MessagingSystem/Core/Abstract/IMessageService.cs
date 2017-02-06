using Domain.Entities;
using System.Collections.Generic;

namespace Core.Abstract
{
    public interface IMessageService
    {
        // Return the user outbox messages.
        IList<Message> GetAllMessagesByUserId(int userId, bool includeDeleted = false);

        // Deletes a message from the outbox.
        int DeleteMessage(Message message);

        // Returns an outbox message by Id.
        Message GetMessageById(int messageId, bool includeDeleted = false);

        // Takes a message object and inserts it into the database.
        int InsertMessage(Message message, IList<User> receivers);
    }
}
