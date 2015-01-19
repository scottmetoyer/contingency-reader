using Reader.Domain;
using Reader.Domain.Configuration;
using Reader.Web.Helpers;
using Reader.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reader.Web.Controllers
{
    public class OptionsController : Controller
    {
        private FeedRepository _repository;
        private FeedServices _services;
        private ViewModelBuilder _builder;

        public OptionsController()
        {
            UserSettingsSection config = (UserSettingsSection)System.Configuration.ConfigurationManager.GetSection("userSettings");
            _repository = new FeedRepository(config.ConnectionString.Value);
            _services = new FeedServices(_repository);
            _builder = new ViewModelBuilder(_repository);
        }

        public ActionResult Index()
        {
            var model = new OptionsViewModel();

            try
            {
                model = _builder.BuildOptionsViewModel();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message.ToString();
            }

            return View(model);
        }

        public ActionResult Purge()
        {
            try
            {
                _repository.PurgeItems();
                TempData["Message"] = "Post data has been cleared";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error clearing post data: " + ex.Message.ToString();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Save(bool autoRefresh)
        {
            try
            {
                var option = _repository.Options.FirstOrDefault(x => x.Key == "AutoRefresh");
                if (option == null)
                {
                    option = new Option() { Key = "AutoRefresh" };
                }
                option.Value = autoRefresh.ToString();

                _repository.SaveOption(option);
                TempData["Message"] = "Options saved";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error saving options: " + ex.Message.ToString();
            }

            return RedirectToAction("Index", new { feed = "options" });
        }
    }
}
