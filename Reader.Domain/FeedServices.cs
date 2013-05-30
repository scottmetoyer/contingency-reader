using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Xml.Linq;

namespace Reader.Domain
{
    public class FeedServices
    {
        public Repository _repository;

        public FeedServices(Repository repository)
        {
            _repository = repository;
        }

        public void Refresh()
        {
            var feeds = _repository.GetFeeds();

            foreach (var feed in feeds)
            {
                var items = this.Fetch(feed.URL);
            }
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

        public List<Item> Fetch(string feedUrl)
        {
            List<Item> items = new List<Item>();

            XmlReader reader = XmlReader.Create(feedUrl);
            Rss20FeedFormatter formatter = new Rss20FeedFormatter();
            formatter.ReadFrom(reader);
            reader.Close();
            var syndicationItems = formatter.Feed.Items.ToList();

            foreach (var i in syndicationItems)
            {
                Item item = new Item();
                item.Title = (i.Title != null) ? i.Title.Text : i.BaseUri.ToString();
                item.PublishDate = (i.PublishDate != null) ? i.PublishDate.ToString("r") : string.Empty;
                
                var link = i.Links.FirstOrDefault(x => x.RelationshipType == "alternate");
                item.URL = (link != null) ? link.Uri.ToString() : feedUrl;

                if (i.Content != null)
                {
                    item.Content = i.Content.ToString();
                }
                else
                {
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

                // No content? Use the summary.
                if (item.Content == string.Empty && i.Summary != null)
                {
                    item.Content = i.Summary.Text;
                }

                items.Add(item);
            }

            return items;
        }
    }
}
