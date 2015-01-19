using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reader.Web.Models
{
    public class OptionsViewModel
    {
        public string ChannelName { get; set; }

        public List<FeedViewModel> Feeds { get; set; }

        public bool AutoRefresh { get; set; }

        public OptionsViewModel()
        {
            this.Feeds = new List<FeedViewModel>();
        }
    }
}