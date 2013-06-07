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

        public bool Selected { get; set; }

        public int UnreadCount { get; set; }

        public FeedViewModel()
        {
            this.Feed = new Feed { URL = string.Empty, FeedID = 0, DisplayName = string.Empty };
        }
    }
}