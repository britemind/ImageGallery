using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageGallery.Controllers
{
    public class AdminController : Controller
    {
        private Models.SiteAdminRepository repository = null;

        public AdminController()
        {
            repository = new Models.SiteAdminRepository();
        }

        public ActionResult Index()
        {
            if (repository.SiteEnabled == true)
            {
                ViewBag.Message = "Site Enabled";
            }
            else
            {
                ViewBag.Message = "Site Disabled";
            }
            return View();
        }
        
        public ActionResult Enable()
        {
            repository.SiteEnabled = true;
            return RedirectToAction("Index");
        }

        public ActionResult Disable()
        {
            repository.SiteEnabled = false;
            return RedirectToAction("Index");
        }
    }
}
