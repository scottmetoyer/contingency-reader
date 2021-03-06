﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Reader.Domain;
using Reader.Domain.Configuration;
using Reader.Web.Helpers;
using Reader.Web.Models;
using System.Web.Caching;

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
                return RedirectToAction("Index", new { feed = feed.URL });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error setting view mode: " + ex.Message.ToString();
            }

            return RedirectToAction("Index");
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

                return RedirectToAction("Index", new { feed = feed.URL });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error marking feed read: " + ex.Message.ToString();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Refresh(int feedId)
        {
            try
            {
                var feed = _repository.Feeds.FirstOrDefault(x => x.FeedID == feedId);
                _services.Fetch(feed);
                TempData["Message"] = feed.DisplayName + " has been refreshed";
                return RedirectToAction("Index", new { feed = feed.URL });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error refreshing feed: " + ex.Message.ToString();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Index(string feed)
        {
            var model = new IndexViewModel();
            model.ChannelURL = feed;

            try
            {
                var selectedFeed = _repository.Feeds.FirstOrDefault(x => x.URL == feed);
               
                if (selectedFeed != null)
                {
                    model.ChannelName = selectedFeed.DisplayName;
                    model.BlogURL = selectedFeed.BlogURL;
                    model.LastRefresh = selectedFeed.LastRefresh.ToString();

                    // Refresh the feed if the option is set
                    if (_services.GetOption("AutoRefresh").Value.ToLower() == "true")
                    {
                        _services.Fetch(selectedFeed);
                        TempData["Message"] = selectedFeed.DisplayName + " has been refreshed";
                    }

                    try
                    {
                        if (Session["ViewMode"] == null || Session["ViewMode"].ToString() == "Show Unread Items")
                        {
                            var items = _repository.Items.Where(x => x.FeedID == selectedFeed.FeedID && x.IsRead == false).OrderByDescending(x => x.ItemID).Take(10).ToList();
                            model.Items = _builder.BuildItemsViewModelList(items, true);
                        }
                        else
                        {
                            var items = _repository.Items.Where(x => x.FeedID == selectedFeed.FeedID).OrderByDescending(x => x.ItemID).Take(10).ToList();
                            model.Items = _builder.BuildItemsViewModelList(items, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Error reading feed: " + ex.Message.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(model.ChannelURL))
                {
                    // Retrieve starred items
                    if (model.ChannelURL.ToLower() == "starred")
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

                    // Show options screen
                    if (model.ChannelURL.ToLower() == "options")
                    {
                        model.ChannelName = "Options";
                    }
                }

                // Build the feeds sidebar list
                var feeds = _repository.Feeds.OrderBy(x => x.DisplayName).ToList();
                model.Feeds = _builder.BuildFeedsViewModelList(feeds, selectedFeed);
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

            if (id > 0)
            {
                model.Item = _repository.Items.FirstOrDefault(x => x.ItemID == id);
                Item nextItem = null;

                bool includeRead = Session["ViewMode"] == null || Session["ViewMode"].ToString() == "Show Unread Items" ? false : true;
                nextItem = _services.GetNextItem(includeRead, id, model.Item.FeedID);

                if (nextItem != null)
                {
                    model.NextItemID = nextItem.ItemID;
                }
                else
                {
                    model.NextItemID = 0;
                }
            }

            return View(model);
        }

        public FileResult FeedImage(int id)
        {
            try
            {
                byte[] data = (byte[])HttpContext.Cache["image_" + id.ToString()];

                if (data == null)
                {
                    var feed = _repository.Feeds.FirstOrDefault(x => x.FeedID == id);
                    data = feed.Favicon.ToArray();
                    HttpContext.Cache["image_" + id.ToString()] = data;
                }

                return File(data, "image/x-icon");
            }
            catch
            {
                return File(Server.MapPath("~/Images/rss_ico.png"), "image/png");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Subscribe(string url)
        {
            if (url.Trim() == string.Empty)
            {
                TempData["Error"] = "URL is required to add a feed";
            }
            else
            {
                try
                {
                    // Check for duplicates
                    var feed = _repository.Feeds.FirstOrDefault(x => x.URL == url);
                    if (feed == null)
                    {
                        feed = new Feed { FeedID = 0, URL = url };
                    }

                    _services.FillFeed(feed);
                    _repository.SaveFeed(feed);

                    TempData["Message"] = feed.DisplayName + " added";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error: " + ex.Message.ToString();
                }
            }

            return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error unsubscribing from feed: " + ex.Message.ToString();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public HttpResponseMessage Star(int id, bool star)
        {
            bool success = false;

            try
            {
                var item = _repository.Items.FirstOrDefault(x => x.ItemID == id);
                if (item != null)
                {
                    item.IsStarred = star;
                    _repository.SaveItem(item);
                    success = true;
                }
            }
            catch
            {
                success = false;
            }

            if (success)
            {
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public HttpResponseMessage Read(int id)
        {
            bool success = false;

            try
            {
                var item = _repository.Items.FirstOrDefault(x => x.ItemID == id);
                if (item != null)
                {
                    item.IsRead = true;
                    _repository.SaveItem(item);
                    success = true;
                }
            }
            catch
            {
                success = false;
            }

            if (success)
            {
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
