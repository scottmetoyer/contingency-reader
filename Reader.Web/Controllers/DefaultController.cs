using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reader.Domain;
using Reader.Domain.Configuration;
using Reader.Web.Models;

namespace Reader.Web.Controllers
{
    [Authorize]
    public class DefaultController : Controller
    {
        private FeedRepository _repository;
        private FeedServices _services;

        public DefaultController()
        {
            UserSettingsSection config = (UserSettingsSection)System.Configuration.ConfigurationManager.GetSection("userSettings");
            _repository = new FeedRepository(config.ConnectionString.Value);
            _services = new FeedServices(_repository);
        }

        public ActionResult View(string feed)
        {
            var model = new ItemsViewModel();

            try
            {
                var feeds = _repository.Feeds.ToList();
                foreach (var f in feeds)
                {
                    var feedViewModel = new FeedViewModel { Feed = f };
                    feedViewModel.UnreadCount = _repository.Items.Count(x => x.FeedID == f.FeedID && x.IsRead == false);
                    model.Feeds.Add(feedViewModel);

                    if (f.URL == feed)
                    {
                        model.SelectedFeed = feedViewModel;
                    }
                }

                if (model.SelectedFeed.Feed.URL != string.Empty)
                {
                    try
                    {
                        model.Items = _repository.Items.Where(x => x.FeedID == model.SelectedFeed.Feed.FeedID).ToList();
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

                    TempData["Message"] = "Feed added";
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
