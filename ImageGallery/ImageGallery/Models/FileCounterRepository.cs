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
    public class FileCounterRepository
    {
        MongoClient client = null;
        MongoServer server = null;
        MongoDatabase database = null;
        MongoCollection FileCounterCollection = null;

        public FileCounterRepository()
        {
            Init();
        }

        public FileCounter Select(string FileId)
        {
            var query = Query.EQ("FileId", ObjectId.Parse(FileId));
            var FileCounter = FileCounterCollection.FindOneAs<FileCounter>(query);
            return FileCounter;
        }

        public void Insert(FileCounter FileCounter)
        {
            FileCounterCollection.Insert(FileCounter);
        }

        public void Update(FileCounter FileCounter)
        {
            FileCounterCollection.Save(FileCounter);
        }

        public void Delete(FileCounter FileCounter)
        {
            var id = FileCounter.id;
            var query = Query.EQ("_id", FileCounter.id);
            FileCounterCollection.Remove(query);
        }

        public List<FileCounter> List()
        {
            return FileCounterCollection.FindAllAs<FileCounter>().ToList();
        }

        private void Init()
        {
            client = new MongoClient(MongoConnectionString());
            server = client.GetServer();
            var databaseName = MongoUrl.Create(MongoConnectionString()).DatabaseName;
            database = server.GetDatabase(databaseName);
            FileCounterCollection = database.GetCollection<FileCounter>("FileCounter");
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