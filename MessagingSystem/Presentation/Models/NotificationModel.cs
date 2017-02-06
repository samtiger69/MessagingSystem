using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Models
{
    public class NotificationModel
    {
        public int Type { get; set; }
        public string Message { get; set; }
    }
}