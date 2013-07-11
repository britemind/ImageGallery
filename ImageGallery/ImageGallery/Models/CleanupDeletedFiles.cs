using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageGallery.Models
{
    public class CleanupDeletedFiles
    {
        public void Init()
        {
            var listOfUsedFiles = new Dictionary<string,string>();

            var fileRepository = new FileRepository();
            
            var galleryRepository = new GalleryRepository();
            var galleryList = galleryRepository.List();

            foreach (var g in galleryList)
            {
                foreach (var i in g.Images)
                {
                    if (listOfUsedFiles.ContainsKey(i.ThumbnailId.ToString()) == false)
                    {
                        listOfUsedFiles.Add(i.ThumbnailId.ToString(), "true");
                    }
                    if (listOfUsedFiles.ContainsKey(i.ImageFileId.ToString()) == false)
                    {
                        listOfUsedFiles.Add(i.ImageFileId.ToString(), "true");
                    }
                }
            }

            var allFiles = fileRepository.SelectAll();

            foreach (var f in allFiles)
            {
                if (listOfUsedFiles.ContainsKey(f.Id.ToString())==false)
                {
                    f.Delete();
                }
            }

        }

    }
}