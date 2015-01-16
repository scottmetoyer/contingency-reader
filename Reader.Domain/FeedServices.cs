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
            var source = GetFeed(HttpUtility.UrlDecode(feed.URL));
            var syndicationItems = source.Items.ToList();

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

                if (IsValidSqlDateTime(i.PublishDate))
                {
                    item.PublishDate = i.PublishDate.DateTime;
                }
                else
                {
                    item.PublishDate = DateTime.Now;
                }

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

                // Otherwise use the summary
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
                    if (!i.Title.ToLower().Contains("sponsored post"))
                    {
                        _repository.SaveItem(i);
                    }
                }
            }

            feed.LastRefresh = DateTime.Now;
            _repository.SaveFeed(feed);
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

        private SyndicationFeed GetFeed(string url)
        {
            using (XmlReader reader = XmlReader.Create(url))
            {
                Rss20FeedFormatter rss = new Rss20FeedFormatter();
                if (rss.CanRead(reader))
                {
                    rss.ReadFrom(reader);
                    return rss.Feed;
                }

                Atom10FeedFormatter atom = new Atom10FeedFormatter();
                if (atom.CanRead(reader))
                {
                    atom.ReadFrom(reader);
                    return atom.Feed;
                }
            }

            return null;
        }

        public void FillFeed(Feed feed)
        {
            SyndicationFeed source = GetFeed(feed.URL);

            if (source == null)
            {
                throw new Exception("Not a valid Atom or RSS feed.");
            }
            var link = source.Links.FirstOrDefault(x => x.RelationshipType == "alternate");
            feed.BlogURL = link.GetAbsoluteUri().AbsoluteUri;
            feed.DisplayName = source.Title.Text;

            if (feed.DisplayName == string.Empty)
            {
                feed.DisplayName = link.GetAbsoluteUri().Host;
            }

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

        private bool IsValidSqlDateTime(DateTimeOffset value)
        {
            bool valid = false;
            if (value == null) return valid;

            // Check for SQL DATETIME validity
            DateTimeOffset minDateTime = new DateTimeOffset(1753, 1, 1, 1, 1, 1, new TimeSpan(0));
            DateTimeOffset maxDateTime = new DateTimeOffset(9999, 12, 31, 23, 59, 59, 997, new TimeSpan(0));

            if (value >= minDateTime && value <= maxDateTime)
            {
                valid = true;
            }

            return valid;
        }
    }
}
