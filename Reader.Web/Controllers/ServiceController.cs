using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reader.Domain;
using Reader.Domain.Configuration;

namespace Reader.Web.Controllers
{
    public class ServiceController : Controller
    {
        private FeedRepository _repository;
        private FeedServices _services;

        public ServiceController()
        {
            UserSettingsSection config = (UserSettingsSection)System.Configuration.ConfigurationManager.GetSection("userSettings");
            _repository = new FeedRepository(config.ConnectionString.Value);
            _services = new FeedServices(_repository);
        }

        public string Refresh()
        {
            string result = string.Empty;

            var feeds = _repository.Feeds.ToList();

            foreach (var feed in feeds)
            {
                try
                {
                    _services.Fetch(feed);
                }
                catch (Exception ex)
                {
                    result += ex.Message.ToString() + "\r\n";
                }
            }

            return result;
        }
    }
}
