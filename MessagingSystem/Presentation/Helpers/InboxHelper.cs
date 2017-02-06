using Core.Abstract;
using Core.Implementation;
using Domain.Entities;
using Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Helpers
{
    public class InboxHelper : BaseHelper
    {
        #region fields
        private IUserMessageService _userMessageService;
        #endregion

        public InboxHelper()
            :base()
        {
            _userMessageService = new UserMessageService();
        }

        public InboxViewModel PrepareInbox(int userId, int? pageNumber)
        {
            var model = new InboxViewModel();

            model.PageNumber = (pageNumber == null ? 1 : Convert.ToInt32(pageNumber));
            model.PageSize = 10;
            var messages = _userMessageService.GetInbox(userId);

            if (messages != null)
            {
                model.Messages = messages.OrderBy(x => x.messageId)
                         .Skip(model.PageSize * (model.PageNumber - 1))
                         .Take(model.PageSize).ToList();

                model.TotalCount = messages.Count();
                var page = (model.TotalCount / model.PageSize) - (model.TotalCount % model.PageSize == 0 ? 1 : 0);
                model.PagerCount = page + 1;
            }
            model.UnreadMessages = GetUnreadMessagesCount(model.Messages);
            return model;
        }

        private int GetUnreadMessagesCount(IList<UserMessage> messages)
        {
            var unreadMessages = messages.Where(m => (m.messageStatus & MessageStatus.Read) != MessageStatus.Read).ToList();
            return unreadMessages.Count;
        }

        public int DeleteUserMessages(User user, int[] ids)
        {
            var deletedCounter = 0;
            foreach (var messageId in ids)
            {
                var message = _userMessageService.GetUserMessage(messageId, user.Id);
                if (message != null)
                {
                    _userMessageService.MoveMessageToTrash(message);
                    deletedCounter++;
                }
            }
            return deletedCounter;
        }

        public UserMessage GetUserMessage(int messageId, int userId)
        {
            var userMessage = _userMessageService.GetUserMessage(messageId, userId);
            if(userMessage != null)
            {
                _userMessageService.MakeMessageRead(userMessage);
            }
            return userMessage;
        }

        public void MoveMessageToTrash(UserMessage message)
        {
            _userMessageService.MoveMessageToTrash(message);
        }
    }
}