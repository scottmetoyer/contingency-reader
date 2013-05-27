using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using Reader.Domain;

namespace Reader.Web.Models
{
    public class FeedViewModel
    {
        public string SelectedFeed = string.Empty;

        public List<Feed> Feeds { get; set; }

        public List<Item> Items { get; set; }

        public FeedViewModel()
        {
            this.Feeds = new List<Feed>();
            this.Items = new List<Item>();
        }
    }
}