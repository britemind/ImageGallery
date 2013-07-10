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
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int HitCount 
        {
            get
            {
                var fileCounterRepository = new FileCounterRepository();
                var fileCounter = fileCounterRepository.Select(this.ImageFileId.ToString());
                if (fileCounter == null)
                {
                    fileCounter = new FileCounter();
                    fileCounter.FileId = this.ImageFileId;
                    fileCounter.Counter = 0;
                    fileCounterRepository.Insert(fileCounter);
                }
                else { }
                return fileCounter.Counter;
            }
        }
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