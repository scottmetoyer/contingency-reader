using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Xml.Linq;
using System.Web;

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

        public bool ValidateUrl(string url)
        {
            bool valid = false;

            try
            {
                XmlReader reader = XmlReader.Create(url);
                Rss20FeedFormatter formatter = new Rss20FeedFormatter();
                formatter.ReadFrom(reader);
                reader.Close();
                valid = true;
            }
            catch
            {
                valid = false;
            }

            return valid;
        }

        public string GetDisplayName(string url)
        {
            string displayName = string.Empty;

            try
            {
                XmlReader reader = XmlReader.Create(url);
                Rss20FeedFormatter formatter = new Rss20FeedFormatter();
                formatter.ReadFrom(reader);
                reader.Close();
                displayName = formatter.Feed.Title.Text;
            }
            catch
            {
                displayName = string.Empty;
            }

            return displayName;
        }

        public void Fetch(Feed feed)
        {
            List<Item> items = new List<Item>();

            XmlReader reader = XmlReader.Create(HttpUtility.UrlDecode(feed.URL));
            Rss20FeedFormatter formatter = new Rss20FeedFormatter();
            formatter.ReadFrom(reader);
            reader.Close();
            var syndicationItems = formatter.Feed.Items.ToList();

            foreach (var i in syndicationItems)
            {
                // An article link is required... this is how we uniquely identify articles.
                var link = i.Links.FirstOrDefault(x => x.RelationshipType == "alternate");
                if (link == null)
                {
                    continue;
                }

                string itemUrl = (HttpUtility.UrlEncode(link.Uri.ToString()));

                // Check if we've already saved this item
                var item = _repository.Items.FirstOrDefault(x => x.URL == itemUrl);

                if (item == null)
                {
                    item = new Item { ItemID = 0 };
                    item.FeedID = feed.FeedID;
                    item.URL = itemUrl;
                    item.IsStarred = false;
                    item.IsRead = false;
                    item.Title = (i.Title != null) ? i.Title.Text : "No title";

                    if (i.PublishDate != null)
                        item.PublishDate = i.PublishDate.DateTime;

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

                    _repository.SaveItem(item);
                }
            }
        }
    }
}
