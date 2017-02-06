using Domain.Entities;
using Presentation.Helpers;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    public class OutboxController : Controller
    {

        #region Fields
        private OutboxHelper _helper;
        private Notifications _notifications;
        #endregion

        // Constructor.
        public OutboxController()
        {
            _helper = new OutboxHelper();
            _notifications = new Notifications(TempData);
        }

        // List all the user outgoing messages.
        public ActionResult Index(int? pageNumber)
        {
            var user = _helper.GetUser(User.Identity.Name);
            var model = _helper.PrepareOutbox(user.Id, pageNumber);

            if (model.Messages.Count == 0)
            {
                _notifications.AddInfoNotification("Outbox is empty");
            }
            return View(model);
        }

        // Deletes all the selected messages in user's outbox.
        [HttpPost]
        public ActionResult Index(int[] Ids)
        {
            var user = _helper.GetUser(User.Identity.Name);
            if (Ids != null)
            {
                var deletedCounter = _helper.DeleteMessages(user.Id, Ids);

                if (deletedCounter == 1)
                {
                    _notifications.AddSuccessNotification(String.Format("{0} message has been deleted successfully.", deletedCounter));
                }
                if (deletedCounter > 1)
                {
                    _notifications.AddSuccessNotification(String.Format("{0} messages have been deleted successfully.", deletedCounter));
                }
                if (deletedCounter != Ids.Length)
                {
                    _notifications.AddErrorNotification(String.Format("{0} messages weren't deleted", Ids.Length - deletedCounter));
                }
            }
            else
            {
                _notifications.AddErrorNotification("Select messages to delete");
            }
            return RedirectToAction("Index");
        }

        // View a user sent message.
        public ActionResult ViewMessage(int messageId)
        {
            var message = _helper.GetMessageById(messageId);

            if(message == null)
            {
                _notifications.AddErrorNotification("Message not found");
                RedirectToAction("Index");
            }

            var user = _helper.GetUser(User.Identity.Name);

            if (message.SenderUserId != user.Id)
            {
                _notifications.AddErrorNotification("You cannot view this message");
                return RedirectToAction("Index");
            }

            return View(message);
        }

        // Delete a user sent message.
        public ActionResult DeleteMessage(int messageId)
        {
            var message = _helper.GetMessageById(messageId);

            if (message == null)
            {
                _notifications.AddErrorNotification("Message not found");
                RedirectToAction("Index");
            }

            var user = _helper.GetUser(User.Identity.Name);

            if (message.SenderUserId == user.Id)
            {
                _notifications.AddErrorNotification("You cannot delete this message");
                
            }

            _helper.DeleteMessage(message);

            _notifications.AddSuccessNotification("Message has been deleted successfully");

            return RedirectToAction("Index");
        }

        // Creates a message
        public ActionResult ComposeMessage(int? Id)
        {
            var user = _helper.GetUser(User.Identity.Name);
            var model = _helper.PrepareComposeMessageModel(user.Id, Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult ComposeMessage(ComposeMessage model, string[] Ids)
        {
            var user = _helper.GetUser(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                model = _helper.PrepareComposeMessageModel(user.Id, model.DefualtReceiver);
                return View(model);
            }
            if (Ids == null)
            {
                model = _helper.PrepareComposeMessageModel(user.Id, model.DefualtReceiver);
                _notifications.AddErrorNotification("Select one receiver at least!");
                return View(model);
            }

            _helper.AddMessage(user.Id, model, Ids);

            _notifications.AddSuccessNotification("Message has been Sent");

            return RedirectToAction("Index");
        }
    }
}