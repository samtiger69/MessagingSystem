using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Helpers
{
    public class Notifications
    {
        public TempDataDictionary Data { get; set; }
        public Notifications(TempDataDictionary _data)
        {
            Data = _data;
        }

        public void AddErrorNotification(string errorMessage)
        {
            var notification = new NotificationModel
            {
                Type = 1,
                Message = errorMessage
            };
            AddNotification(notification);
        }

        public void AddSuccessNotification(string successMessage)
        {
            var notification = new NotificationModel
            {
                Type = 2,
                Message = successMessage
            };
            AddNotification(notification);
        }

        public void AddInfoNotification(string infoMessage)
        {
            var notification = new NotificationModel
            {
                Type = 3,
                Message = infoMessage
            };
            AddNotification(notification);
        }

        public void AddWarningNotification(string warningMessage)
        {
            var notification = new NotificationModel
            {
                Type = 4,
                Message = warningMessage
            };
            AddNotification(notification);
        }

        private void AddNotification(NotificationModel notification)
        {
            var notifications = Data["notes"] as List<NotificationModel>;
            if (notifications == null)
            {
                notifications = new List<NotificationModel>();
            }
            notifications.Add(notification);
            Data["notes"] = notifications;
        }
    }
}