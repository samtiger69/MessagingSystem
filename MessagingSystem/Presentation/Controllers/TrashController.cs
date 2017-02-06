using Presentation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    public class TrashController : Controller
    {

        #region Fields
        private TrashHelper _helper;
        private Notifications _notifications;
        #endregion

        // Constructor.
        public TrashController()
        {
            _helper = new TrashHelper();
            _notifications = new Notifications(TempData);
        }

        // List all the messages in Trashbox.
        public ActionResult Index(int? pageNumber)
        {
            var user = _helper.GetUser(User.Identity.Name);
            var model = _helper.PrepareInbox(user.Id, pageNumber);
            if (model.Messages.Count == 0)
            {
                _notifications.AddInfoNotification("Trash box is empty");
            }
            return View(model);
        }

        // Delete all the selected messages from trashbox.
        [HttpPost]
        public ActionResult Index(int[] Ids)
        {
            var user = _helper.GetUser(User.Identity.Name);
            if(Ids != null)
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
                _notifications.AddErrorNotification("Select Messages to delete");
            }
            return RedirectToAction("Index");
        }

        // View an incoming message.
        public ActionResult ViewMessage(int messageId)
        {
            var user = _helper.GetUser(User.Identity.Name);
            var message = _helper.GetUserMessage(messageId, user.Id);
            if(message != null)
            {
                return View(message);
            }
            else
            {
                _notifications.AddErrorNotification("Message not found");
                return RedirectToAction("Index");
            }
        }

        // Deletes a message from trashbox
        public ActionResult DeleteMessage(int messageId)
        {
            var user = _helper.GetUser(User.Identity.Name);
            var message = _helper.GetUserMessage(messageId, user.Id);
            if(message != null)
            {
                _helper.DeleteUserMessage(message);
                _notifications.AddSuccessNotification("Message has been deleted successfully");
            }
            else
            {
                _notifications.AddErrorNotification("Message not found");
            }
            return RedirectToAction("Index");
        }

        // Moves a trashbox message back to inbox.
        public ActionResult RestoreMessage(int messageId)
        {
            var user = _helper.GetUser(User.Identity.Name);
            var message = _helper.GetUserMessage(messageId, user.Id);
            if(message != null)
            {
                _helper.RestoreUserMessageToInbox(message);
                _notifications.AddSuccessNotification("Message has been restored");
            }
            else
            {
                _notifications.AddErrorNotification("Message not found");
            }
            return RedirectToAction("Index");
        }
    }
}