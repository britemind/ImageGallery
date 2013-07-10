using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageGallery.Models;

namespace ImageGallery.Controllers
{
    public class HomeController : Controller
    {
        private GalleryRepository galleryRepository;
        private FileRepository fileRepository;
        private FileCounterRepository fileCounterRepository;

        public HomeController()
        {
            galleryRepository = new GalleryRepository();
            fileRepository = new FileRepository();
            fileCounterRepository = new FileCounterRepository();
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
            Image.ImageFileId = fileRepository.Insert(stream, file.FileName, file.ContentType);
            stream.Seek(0,0);
            Image.ThumbnailId = GenerateThumbnail(stream, file);
            var Gallery = galleryRepository.Select(id);
            Gallery.Images.Add(Image);
            galleryRepository.Update(Gallery);
            return RedirectToAction("ThumbnailView", new { id = id });
        }

        private MongoDB.Bson.ObjectId GenerateThumbnail(System.IO.Stream stream, HttpPostedFileBase file )
        {
            var ObjectId = new MongoDB.Bson.ObjectId();
            try
            {
                System.Drawing.Image bmp = System.Drawing.Image.FromStream(stream);
                var photo = HomeController.FixedSize(bmp, 128, 128);
                var tempFile = this.HttpContext.ApplicationInstance.Server.MapPath("~/App_Data");
                tempFile += "\\test.jpg";
                photo.Save(tempFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                var fileStream = new System.IO.FileStream(tempFile, System.IO.FileMode.Open);
                var returnValue = fileRepository.Insert(fileStream, file.FileName, file.ContentType);
                fileStream.Close();
                return returnValue;
            }
            catch { }
            return ObjectId;
        }
        
        static System.Drawing.Image FixedSize(System.Drawing.Image imgPhoto, int Width, int Height)
        {
            // Generate Thumbnail
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            System.Drawing.Bitmap bmPhoto = new System.Drawing.Bitmap(Width, Height,
                              System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            System.Drawing.Graphics grPhoto = System.Drawing.Graphics.FromImage(bmPhoto);
            grPhoto.Clear(System.Drawing.Color.White);
            grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new System.Drawing.Rectangle(destX, destY, destWidth, destHeight),
                new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                System.Drawing.GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
        
        public FileContentResult FileContent(string id)
        {

            try
            {
                var fileCounter = fileCounterRepository.Select(id);
                if (fileCounter == null)
                {
                    fileCounter = new FileCounter();
                    fileCounter.FileId = new MongoDB.Bson.ObjectId(id);
                    fileCounter.Counter = 1;
                    fileCounterRepository.Insert(fileCounter);
                }
                else
                {
                    fileCounter.Counter += 1;
                    fileCounterRepository.Update(fileCounter);
                }
            }
            catch
            { }
            
            try
            {
                var file = fileRepository.SelectContent(id);
                var fileInfo = fileRepository.SelectFileInfo(id);
                var fcr = new FileContentResult(file, fileInfo.ContentType);
                return fcr;
            }
            catch
            {
                return null;
            }
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
            var rehydratedGallery = galleryRepository.Select(gallery.id.ToString());
            rehydratedGallery.Name = gallery.Name;
            galleryRepository.Update(rehydratedGallery);
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
