using Presentation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    public class InboxController : Controller
    {

        #region Fields
        private InboxHelper _helper;
        private Notifications _notifications;
        #endregion

        // Constructor.
        public InboxController()
        {
            _notifications = new Notifications(TempData);
            _helper = new InboxHelper();
        }

        // List all the user incoming messages.
        public ActionResult Index(int? pageNumber)
        {
            var user = _helper.GetUser(User.Identity.Name);

            var model = _helper.PrepareInbox(user.Id, pageNumber);

            if (model.Messages.Count == 0)
            {
                _notifications.AddInfoNotification("Your inbox is empty");
            }

            if (model.UnreadMessages == 1)
            {
                _notifications.AddInfoNotification("You have 1 new message");
            }

            if (model.UnreadMessages > 1)
            {
                _notifications.AddInfoNotification(String.Format("You have {0} new messages", model.UnreadMessages));
            }
            return View(model);
        }

        // Moves the selected messages to trashbox.
        [HttpPost]
        public ActionResult Index(int[] Ids)
        {
            var user = _helper.GetUser(User.Identity.Name);

            // Checks if the user didn't check messages to delete.
            if (Ids == null)
            {
                _notifications.AddErrorNotification("Select Messages to delete");
                return RedirectToAction("Index");
            }

            // Deletes the messages from user inbox.
            var deletedCounter = _helper.DeleteUserMessages(user, Ids);

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
            return RedirectToAction("Index");
        }

        // View a user inbox message.
        public ActionResult ViewMessage(int messageId)
        {
            var user = _helper.GetUser(User.Identity.Name);

            var message = _helper.GetUserMessage(messageId, user.Id);

            if (message == null)
            {
                _notifications.AddErrorNotification("Message not found");
                return RedirectToAction("Index");
            }

            return View(message);
        }

        // Move a user inbox message to trashbox.
        public ActionResult TrashMessage(int messageId)
        {
            var user = _helper.GetUser(User.Identity.Name);
            var message = _helper.GetUserMessage(messageId, user.Id);

            if (message == null)
            {
                _notifications.AddErrorNotification("Message not found");
                return RedirectToAction("Index");
            }
            _helper.MoveMessageToTrash(message);
            _notifications.AddSuccessNotification("1 Message has been deleted successfully");
            return RedirectToAction("Index");
        }
    }
}