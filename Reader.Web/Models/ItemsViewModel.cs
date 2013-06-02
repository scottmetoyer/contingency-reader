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
        public string SelectedFeedURL { get; set; }

        public string SelectedFeedName { get; set; }

        public List<FeedViewModel> Feeds { get; set; }

        public List<Item> Items { get; set; }

        public ItemsViewModel()
        {
            this.Feeds = new List<FeedViewModel>();
            this.Items = new List<Item>();
        }
    }
}