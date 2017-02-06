using Core.Abstract;
using Core.Implementation;
using Domain.Entities;
using Presentation.Models;
using Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Helpers
{
    public class OutboxHelper : BaseHelper
    {
        #region Fields
        private IUserService _userService;
        private IMessageService _messageService;
        #endregion

        public OutboxHelper()
            :base()
        {
            _messageService = new MessageService();
            _userService = new UserService();
        }

        public OutboxViewModel PrepareOutbox(int userId, int? pageNumber)
        {
            var model = new OutboxViewModel();
            model.PageNumber = (pageNumber == null ? 1 : Convert.ToInt32(pageNumber));
            model.PageSize = 10;
            var messages = _messageService.GetAllMessagesByUserId(userId);

            if (messages != null)
            {
                model.Messages = messages.OrderBy(x => x.Id)
                         .Skip(model.PageSize * (model.PageNumber - 1))
                         .Take(model.PageSize).ToList();

                model.TotalCount = messages.Count();
                var page = (model.TotalCount / model.PageSize) - (model.TotalCount % model.PageSize == 0 ? 1 : 0);
                model.PagerCount = page + 1;
            }

            return model;
        }

        public int DeleteMessages(int userId, int[] ids)
        {
            var deletedCounter = 0;
            foreach (var messageId in ids)
            {
                var message = _messageService.GetMessageById(messageId);
                if (message != null && message.SenderUserId == userId)
                {
                    _messageService.DeleteMessage(message);
                    deletedCounter++;
                }
            }
            return deletedCounter;
        }

        public Message GetMessageById(int messageId)
        {
            return _messageService.GetMessageById(messageId);
        }

        public void DeleteMessage(Message message)
        {
            _messageService.DeleteMessage(message);
        }

        public ComposeMessage PrepareComposeMessageModel(int userId, int? defaultRecieverId)
        {
            var model = new ComposeMessage();
            model.Users = _userService.GetUsersBySenderId(userId);
            if (defaultRecieverId.HasValue && model.Users.FirstOrDefault(m=>m.Id == defaultRecieverId) != null)
            {
                model.DefualtReceiver = defaultRecieverId;
            }
            return model;
        }

        public void AddMessage(int userId, ComposeMessage model, string[] Receiversnames)
        {
            var message = new Message
            {
                Title = model.Subject,
                Content = model.Content,
                SendDate = DateTime.Now,
                isDeleted = false,
                SenderUserId = userId
            };
            var receivers = new List<User>();
            foreach (var receiver in Receiversnames)
            {
                var receiverUser = _userService.GetUserByUsername(receiver);
                if (receiverUser != null)
                {
                    receivers.Add(receiverUser);
                }
            }
            var messageId = _messageService.InsertMessage(message, receivers);
        }
    }
}