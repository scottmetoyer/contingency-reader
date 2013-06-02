using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reader.Domain;

namespace Reader.Web.Models
{
    public class FeedViewModel
    {
        public Feed Feed { get; set; }

        public int UnreadCount { get; set; }
    }
}