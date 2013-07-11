using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Builders;
using System.Configuration;
using System.IO;
namespace ImageGallery.Models
{
    public class FileRepository
    {
        MongoClient client = null;
        MongoServer server = null;
        MongoDatabase database = null;
        MongoCollection GalleryCollection = null;

        public FileRepository()
        {
            Init();
        }

        public ObjectId Insert(Stream stream, string fileName, string contentType)
        {
            var gridFsInfo = database.GridFS.Upload(stream, fileName, new MongoDB.Driver.GridFS.MongoGridFSCreateOptions() { ContentType = contentType });
            
            return gridFsInfo.Id.AsObjectId;
        }

        public ObjectId Update(Stream stream, string fileName)
        {
            var gridFsInfo = database.GridFS.Upload(stream, fileName);
            return gridFsInfo.Id.AsObjectId;
        }

        public MongoDB.Driver.GridFS.MongoGridFSFileInfo SelectFileInfo(string fileId)
        {
            ObjectId oid = new ObjectId(fileId);
            var file = database.GridFS.FindOne(Query.EQ("_id", oid));
            return file;
        }

        public List<MongoDB.Driver.GridFS.MongoGridFSFileInfo> SelectAll()
        {
            var file = database.GridFS.FindAll().ToList();
            return file;
        }


        public byte[] SelectContent(string fileId)
        {
            ObjectId oid = new ObjectId(fileId);
            var file = database.GridFS.FindOne(Query.EQ("_id", oid));

            if (file != null)
            {
                using (var stream = file.OpenRead())
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    return bytes;
                }
            }
            else
            {
                return null;
            }
        }


        public void Delete(string fileId)
        {
            var id = new ObjectId(fileId);
            var query = Query.EQ("_id", id);
            GalleryCollection.Remove(query);
        }

        private void Init()
        {
            client = new MongoClient(MongoConnectionString());
            server = client.GetServer();
            var databaseName = MongoUrl.Create(MongoConnectionString()).DatabaseName;
            database = server.GetDatabase(databaseName);
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