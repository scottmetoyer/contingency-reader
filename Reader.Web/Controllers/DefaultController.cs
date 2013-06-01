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
            var model = new FeedViewModel();

            try
            {
                model.Feeds = _repository.Feeds.ToList();

                var selectedFeed = _repository.Feeds.FirstOrDefault(x => x.URL == feed);
                model.SelectedFeedURL = selectedFeed != null ? selectedFeed.URL : string.Empty;

                if (selectedFeed != null)
                {
                    try
                    {
                        model.Items = _repository.Items.Where(x => x.FeedID == selectedFeed.FeedID).ToList();
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
