using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Reader.Domain.Configuration;

namespace Reader.Web.Helpers
{
    public class TokenAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                var authHeader = actionContext.Request.Headers.FirstOrDefault(h => h.Key.Equals("Authorization"));
                if (authHeader.Value == null)
                    return false;

                string decodedToken = authHeader.Value.First();
                UserSettingsSection config = (UserSettingsSection)System.Configuration.ConfigurationManager.GetSection("userSettings");

                if (decodedToken == config.AuthToken.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}