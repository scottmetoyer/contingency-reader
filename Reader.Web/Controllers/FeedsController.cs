using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Reader.Domain;
using Reader.Domain.Configuration;

namespace Reader.Web.Controllers
{
    public class FeedsController : ApiController
    {
        private FeedRepository _repository;
        private FeedServices _services;

        public FeedsController()
        {
            UserSettingsSection config = (UserSettingsSection)System.Configuration.ConfigurationManager.GetSection("userSettings");
            _repository = new FeedRepository(config.ConnectionString.Value);
            _services = new FeedServices(_repository);
        }

        public IEnumerable<Feed> Get()
        {
            return _repository.Feeds;
        }

        public Feed Get(int id)
        {
            var feed = _repository.Feeds.FirstOrDefault(x => x.FeedID == id);

            if (feed == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return feed;
        }

        public HttpResponseMessage Refresh(int id)
        {
            bool success = false;

            try
            {
                var feed = _repository.Feeds.FirstOrDefault(x => x.FeedID == id);
                if (feed != null)
                {
                    _services.Fetch(feed);
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
