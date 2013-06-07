using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reader.Domain;
using Reader.Domain.Configuration;
using Reader.Web.Helpers;
using Reader.Web.Models;

namespace Reader.Web.Controllers
{
    [Authorize]
    public class DefaultController : Controller
    {
        private FeedRepository _repository;
        private FeedServices _services;
        private ViewModelBuilder _builder;

        public DefaultController()
        {
            UserSettingsSection config = (UserSettingsSection)System.Configuration.ConfigurationManager.GetSection("userSettings");
            _repository = new FeedRepository(config.ConnectionString.Value);
            _services = new FeedServices(_repository);
            _builder = new ViewModelBuilder(_repository);
        }

        public ActionResult SetViewMode(string mode, int feedId)
        {
            try
            {
                var feed = _repository.Feeds.FirstOrDefault(x => x.FeedID == feedId);
                Session["ViewMode"] = mode;
                return RedirectToAction("View", new { feed = feed.URL });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error setting view mode: " + ex.Message.ToString();
            }

            return RedirectToAction("View");
        }

        public ActionResult MarkAllRead(int feedId)
        {
            try
            {
                var feed = _repository.Feeds.FirstOrDefault(x => x.FeedID == feedId);
                var items = _repository.Items.Where(x => x.FeedID == feedId && x.IsRead == false);

                foreach (var item in items)
                {
                    item.IsRead = true;
                }

                _repository.SaveChanges();

                return RedirectToAction("View", new { feed = feed.URL });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error marking feed read: " + ex.Message.ToString();
            }

            return RedirectToAction("View");
        }

        public ActionResult Unsubscribe(int feedId)
        {
            try
            {
                var feed = _repository.Feeds.FirstOrDefault(x => x.FeedID == feedId);
                var items = _repository.Items.Where(x => x.FeedID == feed.FeedID && x.IsStarred == false);
                _repository.DeleteFeed(feed);
                _repository.DeleteItems(items);

                TempData["Message"] = "You have been unsubscribed from " + feed.DisplayName;
                return RedirectToAction("View");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error unsubscribing from feed: " + ex.Message.ToString();
            }

            return RedirectToAction("View");
        }

        public ActionResult Refresh(int feedId)
        {
            try
            {
                var feed = _repository.Feeds.FirstOrDefault(x => x.FeedID == feedId);
                _services.Fetch(feed);
                TempData["Message"] = feed.DisplayName + " has been refreshed";
                return RedirectToAction("View", new { feed = feed.URL });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error refreshing feed: " + ex.Message.ToString();
            }

            return RedirectToAction("View");
        }

        public ActionResult View(string feed)
        {
            var model = new ItemsViewModel();
            model.ChannelURL = feed;

            try
            {
                var selectedFeed = _repository.Feeds.FirstOrDefault(x => x.URL == feed);
                var feeds = _repository.Feeds.ToList();
                model.Feeds = _builder.BuildFeedsViewModelList(feeds, selectedFeed);
              
                if (selectedFeed != null)
                {
                    model.ChannelName = selectedFeed.DisplayName;

                    try
                    {
                        if (Session["ViewMode"] == null || Session["ViewMode"].ToString() == "Show Unread Items")
                        {
                            var items = _repository.Items.Where(x => x.FeedID == selectedFeed.FeedID && x.IsRead == false).OrderByDescending(x => x.PublishDate).Take(3).ToList();
                            model.Items = _builder.BuildItemsViewModelList(items, true);
                        }
                        else
                        {
                            var items = _repository.Items.Where(x => x.FeedID == selectedFeed.FeedID).OrderByDescending(x => x.PublishDate).Take(3).ToList();
                            model.Items = _builder.BuildItemsViewModelList(items, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Error reading feed: " + ex.Message.ToString();
                    }
                }

                // Retrieve starred items
                if (string.Compare(model.ChannelURL, "starred", true) >= 0)
                {
                    model.ChannelName = "Starred Items";

                    try
                    {
                        var items = _repository.Items.Where(x => x.IsStarred == true).OrderByDescending(x => x.PublishDate).ToList();
                        model.Items = _builder.BuildItemsViewModelList(items, false);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Error reading feed: " + ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message.ToString();
            }

            return View(model);
        }

        public ActionResult Item(int id)
        {
            ItemViewModel model = new ItemViewModel();
            model.Item = _repository.Items.FirstOrDefault(x => x.ItemID == id);
            Item nextItem = null;

            bool includeRead = Session["ViewMode"] == null || Session["ViewMode"].ToString() == "Show Unread Items" ? false : true;
            nextItem = _services.GetNextItem(includeRead, id, model.Item.FeedID);

            if (nextItem != null)
            {
                model.NextItemID = nextItem.ItemID;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddFeed(string url)
        {
            if (url.Trim() == string.Empty)
            {
                TempData["Error"] = "URL is required to add a feed.";
            }
            else if (!_services.ValidateUrl(url))
            {
                TempData["Error"] = "The URL you entered is not a valid RSS feed.";
            }
            else
            {
                try
                {
                    var feed = new Feed { FeedID = 0 };
                    feed.URL = url;
                    feed.DisplayName = _services.GetDisplayName(url);
                    _repository.SaveFeed(feed);

                    TempData["Message"] = feed.DisplayName + " added";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message.ToString();
                }
            }

            return RedirectToAction("View");
        }
    }
}
