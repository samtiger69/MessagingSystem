using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.ViewModel
{
    public class InboxViewModel
    {
        public InboxViewModel()
        {
            Messages = new List<UserMessage>();
        }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int PagerCount { get; set; }
        public int UnreadMessages { get; set; }
        public virtual IList<UserMessage> Messages { get; set; }
    }
}