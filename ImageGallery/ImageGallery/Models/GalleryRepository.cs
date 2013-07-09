using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Builders;
using System.Configuration;

namespace ImageGallery.Models
{
    public class GalleryRepository
    {
        MongoClient client = null;
        MongoServer server = null;
        MongoDatabase database = null;
        MongoCollection GalleryCollection = null;

        public GalleryRepository()
        {
            Init();
        }

        public Gallery Select(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var gallery = GalleryCollection.FindOneAs<Gallery>(query);
            return gallery;
        }

        public void Insert(Gallery gallery)
        {
            GalleryCollection.Insert(gallery);
        }

        public void Update(Gallery gallery)
        {
            GalleryCollection.Save(gallery);
        }

        public void Delete(Gallery gallery)
        {
            var id = gallery.id;
            var query = Query.EQ("_id", gallery.id);
            GalleryCollection.Remove(query); 
        }

        public List<Gallery> List()
        {
            return GalleryCollection.FindAllAs<Gallery>().ToList();
        }
        
        private void Init()
        {
            client = new MongoClient(MongoConnectionString());
            server = client.GetServer();
            var databaseName = MongoUrl.Create(MongoConnectionString()).DatabaseName;
            database = server.GetDatabase(databaseName);
            GalleryCollection = database.GetCollection<Gallery>("Gallery");
        }

        private string MongoConnectionString()
        {
            var r = ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
                ConfigurationManager.AppSettings.Get("MONGOLAB_URI") ??
                "mongodb://192.168.1.101/Things";
            return r;
        }

    }
}