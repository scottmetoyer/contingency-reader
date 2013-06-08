using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Reader.Domain.Configuration;

namespace Reader.Web.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            UserSettingsSection config = (UserSettingsSection)System.Configuration.ConfigurationManager.GetSection("userSettings");

            if (config == null)
            {
                TempData["Error"] = "Configuration file not found. Did you rename Default.config to User.config?";
            }

            if (username == config.Account.Username && password == config.Account.Password)
            {
                FormsAuthentication.SetAuthCookie(username, true);
                return RedirectToAction("View", "Default");
            }
            else
            {
                TempData["Error"] = "Invalid Username or Password!";
                return View();
            }
        }
    }
}
