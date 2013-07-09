using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageGallery.Models;

namespace ImageGallery.Controllers
{
    public class GalleryController : Controller
    {
        private GalleryRepository galleryRepository;
        private FileRepository fileRepository;

        public GalleryController()
        {
            galleryRepository = new GalleryRepository();
            fileRepository = new FileRepository();
        }

        public ActionResult Index()
        {
            return View(galleryRepository.List());
        }

        public ActionResult ThumbnailView(string id)
        {
            var gallery = galleryRepository.Select(id);
            return View(gallery);
        }

        public ActionResult AddFile(string id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult AddFile(string id, HttpPostedFileBase file)
        {

            var stream = file.InputStream;
            var Image = new Image();
            Image.ImageFileId = fileRepository.Insert(stream, file.FileName);
            
            stream.Seek(0,0);
            //var bytes = new byte[stream.Length];
            //stream.Read(bytes, 0, (int)stream.Length);

            //var f = System.Drawing.Image.FromStream(stream);
            try
            {
                System.Drawing.Image bmp = System.Drawing.Image.FromStream(stream);
                System.Drawing.Image photo = new System.Drawing.Bitmap(32, 32);
                var graphic = System.Drawing.Graphics.FromImage(photo);
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphic.DrawImage(bmp, 0, 0, 64, 64);


                var tempFile = this.HttpContext.ApplicationInstance.Server.MapPath("~/App_Data");
                tempFile += "\\test.jpg";
                photo.Save(tempFile, System.Drawing.Imaging.ImageFormat.Jpeg);

                var fileStream = new System.IO.FileStream(tempFile, System.IO.FileMode.Open);

                Image.ThumbnailId = fileRepository.Insert(fileStream, file.FileName);

                fileStream.Close();
            }
            catch { }

            var Gallery = galleryRepository.Select(id);
            Gallery.Images.Add(Image);
            galleryRepository.Update(Gallery);


            return RedirectToAction("ThumbnailView", new { id = id });

        }

        public FileContentResult FileContent(string id)
        {
            var file = fileRepository.Select(id);
            var fcr = new FileContentResult(file, "application/octet-stream");
            return fcr;
        }
        
        public ActionResult Create()
        {
            return View("Edit");
        }

        [HttpPost]
        public ActionResult Create(Gallery gallery)
        {
            galleryRepository.Insert(gallery);
            return RedirectToAction("Index");
        }
        
        public ActionResult Details(string id)
        {
            return View(galleryRepository.Select(id));
        }


        public ActionResult Delete(string id)
        {
            return View(galleryRepository.Select(id));
        }

        [HttpPost]
        public ActionResult Delete(Gallery gallery)
        {

            galleryRepository.Delete(gallery);
            return RedirectToAction("Index");
        }



        public ActionResult Edit(string id)
        {
            return View(galleryRepository.Select(id));
        }

        [HttpPost]
        public ActionResult Edit(Gallery gallery)
        {
            galleryRepository.Update(gallery);
            return RedirectToAction("Index");
        }
        
        public ActionResult NewGallery()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewGallery(Gallery gallery)
        {
            galleryRepository.Insert(gallery);
            return RedirectToAction("Index");
        }

        public ActionResult ViewGallery()
        {
            return View();
        }
    }
}
