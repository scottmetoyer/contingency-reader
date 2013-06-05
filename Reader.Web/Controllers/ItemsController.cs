using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Reader.Domain;
using Reader.Domain.Configuration;

namespace Reader.Web.Controllers
{
    public class ItemsController : ApiController
    {
        private FeedRepository _repository;
        private FeedServices _services;

        public ItemsController()
        {
            UserSettingsSection config = (UserSettingsSection)System.Configuration.ConfigurationManager.GetSection("userSettings");
            _repository = new FeedRepository(config.ConnectionString.Value);
            _services = new FeedServices(_repository);
        }

        public HttpResponseMessage SetStar([FromBody]int id)
        {
            bool success = false;

            try
            {
                var item = _repository.Items.FirstOrDefault(x => x.ItemID == id);
                if (item != null)
                {
                    item.IsStarred = true;
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

        public HttpResponseMessage RemoveStar([FromBody]int id)
        {
            bool success = false;

            try
            {
                var item = _repository.Items.FirstOrDefault(x => x.ItemID == id);
                if (item != null)
                {
                    item.IsStarred = false;
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

        public HttpResponseMessage Read([FromBody]int id)
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
