using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Xml.Linq;
using System.Web;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace Reader.Domain
{
    public class FeedServices
    {
        public class RefreshResult
        {
            public bool Success { get; set; }

            public string ErrorMessage { get; set; }
        }

        public FeedRepository _repository;

        public FeedServices(FeedRepository repository)
        {
            _repository = repository;
        }

        public void Fetch(Feed feed)
        {
            XmlReader reader = XmlReader.Create(HttpUtility.UrlDecode(feed.URL));
            Rss20FeedFormatter formatter = new Rss20FeedFormatter();
            formatter.ReadFrom(reader);
            reader.Close();
            var syndicationItems = formatter.Feed.Items.ToList();

            var items = new List<Item>();
            foreach (var i in syndicationItems)
            {
                // An article link is required... this is how we uniquely identify articles.
                var link = i.Links.FirstOrDefault(x => x.RelationshipType == "alternate");
                if (link == null)
                {
                    continue;
                }

                string itemUrl = (link.Uri.ToString());

                var item = new Item { ItemID = 0 };
                item.FeedID = feed.FeedID;
                item.URL = itemUrl;
                item.IsStarred = false;
                item.IsRead = false;
                item.Title = (i.Title != null) ? i.Title.Text : "No title";

                if (i.PublishDate != null)
                {
                    item.PublishDate = i.PublishDate.DateTime;
                }
                else
                {
                    item.PublishDate = DateTime.Now;
                }

                if (i.Content != null)
                {
                    item.Content = i.Content.ToString();
                }
                else
                {
                    // The content may be encoded. Go find the encoded content XML node and get the content.
                    StringBuilder sb = new StringBuilder();
                    foreach (SyndicationElementExtension extension in i.ElementExtensions)
                    {
                        XElement e = extension.GetObject<XElement>();
                        if (e.Name.LocalName == "encoded" && e.Name.Namespace.ToString().Contains("content"))
                        {
                            sb.Append(e.Value);
                        }
                    }

                    item.Content = sb.ToString();
                }

                // Last resort, use the summary
                if (item.Content == string.Empty && i.Summary != null)
                {
                    item.Content = i.Summary.Text;
                }

                item.Content = this.ScrubScripts(item.Content);

                items.Add(item);
            }

            // Order the list and save it to the database
            foreach (var i in items.OrderBy(x => x.PublishDate))
            {
                // Check if we've already saved this item
                if (!_repository.Items.Any(x => x.URL == i.URL))
                {
                    i.FetchDate = DateTime.Now;

                    // Don't save advertisements
                    if (string.Compare(i.Title, "sponsored post", true) < 0)
                    {
                        _repository.SaveItem(i);
                    }
                }
            }
        }

        public string ScrubScripts(string fragment)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(fragment);
            doc.DocumentNode.Descendants()
                            .Where(n => n.Name == "script")
                            .ToList()
                            .ForEach(n => n.Remove());
            return doc.DocumentNode.OuterHtml;
        }

        public Item GetNextItem(bool includeRead, int itemId, int feedId)
        {
            Item nextItem = null;
            var item = _repository.Items.FirstOrDefault(x => x.ItemID == itemId);
            if (includeRead)
            {
                nextItem = _repository.Items.OrderByDescending(x => x.ItemID).FirstOrDefault(x => x.FeedID == feedId && x.ItemID < item.ItemID);
            }
            else
            {
                nextItem = _repository.Items.OrderByDescending(x => x.ItemID).FirstOrDefault(x => x.FeedID == feedId && x.IsRead == false && x.ItemID < item.ItemID);
            }

            return nextItem;
        }

        public void FillFeed(Feed feed)
        {
            XmlReader reader = XmlReader.Create(feed.URL);
            Rss20FeedFormatter formatter = new Rss20FeedFormatter();
            formatter.ReadFrom(reader);
            reader.Close();
            feed.DisplayName = formatter.Feed.Title.Text;
       
            var link = formatter.Feed.Links.FirstOrDefault(x => x.RelationshipType == "alternate");

            if (link != null)
            {
                feed.BlogURL = link.GetAbsoluteUri().AbsoluteUri;

                // Load the favicon
                byte[] bytes = new byte[0];
                var imageAddress = "http://" + link.Uri.Host + "/favicon.ico";

                try
                {
                    WebClient client = new WebClient();
                    MemoryStream stream = new MemoryStream(client.DownloadData(imageAddress));
                    feed.Favicon = stream.ToArray();
                }
                catch
                {
                    // We'll let it pass if we can't find the image
                }
            }
        }
    }
}
