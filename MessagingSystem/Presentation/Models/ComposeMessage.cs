using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Presentation.Models
{
    public class ComposeMessage
    {
        public ComposeMessage()
        {
            Users = new List<User>();
        }

        [Required]
        public string Subject { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public int? DefualtReceiver { get; set; }

        public virtual IList<User> Users { get; set; }
    }
}