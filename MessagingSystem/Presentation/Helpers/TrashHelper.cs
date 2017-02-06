using Core.Abstract;
using Core.Implementation;
using Domain.Entities;
using Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Helpers
{
    public class TrashHelper : BaseHelper
    {
        #region Fields
        private IUserMessageService _userMessageService;
        #endregion

        public TrashHelper()
            : base()
        {
            _userMessageService = new UserMessageService();
        }


        public InboxViewModel PrepareInbox(int userId, int? pageNumber)
        {
            var model = new InboxViewModel();

            model.PageNumber = (pageNumber == null ? 1 : Convert.ToInt32(pageNumber));
            model.PageSize = 10;
            var messages = _userMessageService.GetTrashUserMessagesByUserId(userId);

            if (messages != null)
            {
                model.Messages = messages.OrderBy(x => x.messageId)
                         .Skip(model.PageSize * (model.PageNumber - 1))
                         .Take(model.PageSize).ToList();

                model.TotalCount = messages.Count();
                var page = (model.TotalCount / model.PageSize) - (model.TotalCount % model.PageSize == 0 ? 1 : 0);
                model.PagerCount = page + 1;
            }
            return model;
        }

        public int DeleteMessages(int userId, int[] Ids)
        {
            var deletedCounter = 0;
            foreach (var messageId in Ids)
            {
                var message = _userMessageService.GetUserMessage(messageId, userId);
                if (message != null)
                {
                    _userMessageService.DeleteUserMessage(message);
                    deletedCounter++;
                }
            }
            return deletedCounter;
        }

        public UserMessage GetUserMessage(int messageId, int userId)
        {
            var message = _userMessageService.GetUserMessage(messageId, userId);
            if (message != null)
            {
                if ((message.messageStatus & MessageStatus.Read) != MessageStatus.Read)
                {
                    _userMessageService.MakeMessageRead(message);
                }
                return message;
            }

            return null;
        }

        public void DeleteUserMessage(UserMessage message)
        {
            _userMessageService.DeleteUserMessage(message);
        }

        public void RestoreUserMessageToInbox(UserMessage message)
        {
            _userMessageService.RestoreUserMessageToInbox(message);
        }
    }
}