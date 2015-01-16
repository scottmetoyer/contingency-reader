using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reader.Domain;
using Reader.Web.Models;

namespace Reader.Web.Helpers
{
    public class ViewModelBuilder
    {
        private FeedRepository _repository;
        private FeedServices _services;

        public ViewModelBuilder(FeedRepository repository)
        {
            _repository = repository;
            _services = new FeedServices(repository);
        }

        public List<ItemViewModel> BuildItemsViewModelList(List<Item> items, bool lazyLoad)
        {
            List<ItemViewModel> models = new List<ItemViewModel>();

            foreach (var i in items)
            {
                var itemViewModel = new ItemViewModel { Item = i };

                if (lazyLoad)
                {
                    Item nextItem = null;
                    bool includeRead = HttpContext.Current.Session["ViewMode"] == null || HttpContext.Current.Session["ViewMode"].ToString() == "Show Unread Items" ? false : true;
                    nextItem = _services.GetNextItem(includeRead, i.ItemID, i.FeedID);

                    if (nextItem != null)
                    {
                        itemViewModel.NextItemID = nextItem.ItemID;
                    }
                    else
                    {
                        itemViewModel.NextItemID = 0;
                    }
                }

                models.Add(itemViewModel);
            }

            return models;
        }

        public List<FeedViewModel> BuildFeedsViewModelList(List<Feed> feeds, Feed selectedFeed)
        {
            List<FeedViewModel> models = new List<FeedViewModel>();

            foreach (var f in feeds)
            {
                var feedViewModel = new FeedViewModel { Feed = f };
                feedViewModel.UnreadCount = _repository.Items.Count(x => x.FeedID == f.FeedID && x.IsRead == false);

                if (selectedFeed != null && f.FeedID == selectedFeed.FeedID)
                {
                    feedViewModel.Selected = true;
                }

                models.Add(feedViewModel);
            }

            return models;
        }
    }
}