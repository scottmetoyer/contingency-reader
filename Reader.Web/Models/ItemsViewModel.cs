using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using Reader.Domain;

namespace Reader.Web.Models
{
    public class ItemsViewModel
    {
        public string ChannelName { get; set; }

        public string ChannelURL { get; set; }

        public string BlogURL { get; set; }

        public string LastRefresh { get; set; }

        public List<FeedViewModel> Feeds { get; set; }

        public List<ItemViewModel> Items { get; set; }

        public ItemsViewModel()
        {
            this.Feeds = new List<FeedViewModel>();
            this.Items = new List<ItemViewModel>();
            this.LastRefresh = string.Empty;
            this.BlogURL = string.Empty;
            this.ChannelURL = string.Empty;
            this.ChannelName = string.Empty;
        }
    }
}