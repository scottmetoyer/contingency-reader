using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reader.Domain;
using Reader.Web.Models;

namespace Reader.Web.Controllers
{
    [Authorize]
    public class DefaultController : Controller
    {
        private Repository _repository;
        private FeedServices _services;

        public DefaultController()
        {
            _repository = new Repository("StorageConnectionString");
            _services = new FeedServices(_repository);
        }

        public ActionResult View(string feed)
        {
            var model = new FeedViewModel();

            try
            {
                model.Feeds = _repository.GetFeeds();
                model.SelectedFeed = feed;

                if (!string.IsNullOrEmpty(feed))
                {
                    try
                    {
                        model.Items = _services.Fetch(Server.UrlDecode(feed));
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
                    var feed = new Feed(Server.UrlEncode(url));
                    feed.DisplayName = Server.UrlEncode(_services.GetDisplayName(url));
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

        [Authorize]
        public ActionResult DeleteFeeds()
        {
            try
            {
                _repository.DropTable();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message.ToString();
            }

            return RedirectToAction("View");
        }

    }
}
