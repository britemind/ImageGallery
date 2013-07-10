using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Builders;

namespace ImageGallery.Models
{
    public class Gallery
    {
        public ObjectId id { get; set; }
        public string Name { get; set; }
        public List<Image> Images { get; set; }
        public Gallery()
        {
            Images = new List<Image>();
        }
    }

    public class Image
    {
        public ObjectId id { get; set; }
        public ObjectId ThumbnailId { get; set; }
        public ObjectId ImageFileId { get; set; }
    }

    public class Widget
    {
        public ObjectId id { get; set; }
        public string Name { get; set; }
    }

    public class FileCounter
    {
        public ObjectId id { get; set; }
        public ObjectId FileId { get; set; }
        public int Counter { get; set; }
    }
}