using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.ViewModel
{
    public class OutboxViewModel
    {
        public OutboxViewModel()
        {
            Messages = new List<Message>();
        }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int PagerCount { get; set; }
        public virtual IList<Message> Messages { get; set; }
    }
}