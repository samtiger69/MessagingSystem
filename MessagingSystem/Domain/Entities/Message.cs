using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime SendDate { get; set; }
        public bool isDeleted { get; set; }
        public int SenderUserId { get; set; }
        public virtual User Sender { get; set; }
        public virtual IList<User> messageReceivers { get; set; }
    }
}
