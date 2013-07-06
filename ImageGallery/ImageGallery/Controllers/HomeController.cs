using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageGallery.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Builders;

namespace ImageGallery.Controllers
{
    public class HomeController : Controller
    {
        private MongoRepository repository;

        public HomeController()
        {
            repository = new MongoRepository();
        }

        public ActionResult Index()
        {
            return View(repository.Widget.FindAllAs<Widget>().ToList());
        }

        public ActionResult CreateWidget()
        {
            var widget = new Widget();
            widget.Name = DateTime.Now.ToString();
            repository.Widget.Insert(widget);
            return RedirectToAction("Index");
        }

        public ActionResult RemoveWidget()
        {
            var widget = repository.Widget.FindAllAs<Widget>().First();
            var query = Query<Widget>.EQ(e => e.id, widget.id);
            repository.Widget.Remove(query);
            return RedirectToAction("Index");
        }

    }
}
