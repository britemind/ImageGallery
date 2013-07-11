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
    public class SiteAdminRepository
    {
        MongoClient client = null;
        MongoServer server = null;
        MongoDatabase database = null;
        MongoCollection SiteAdminCollection = null;

        public SiteAdminRepository()
        {
            Init();
        }

        public bool SiteEnabled 
        {
            get
            {
                SiteAdmin siteAdmin = null;

                try
                {
                    siteAdmin = SiteAdminCollection.FindAllAs<SiteAdmin>().ToList().First();
                }
                catch { }
                
                if (siteAdmin == null)
                {
                    return true;
                }
                else
                {
                    return siteAdmin.SiteEnabled;
                }
            }
            set
            {
                SiteAdminCollection.RemoveAll();
                {
                    var siteAdmin = new SiteAdmin();
                    siteAdmin.SiteEnabled = value;
                    SiteAdminCollection.Insert(siteAdmin);
                }
            }
        }

        private void Init()
        {
            client = new MongoClient(MongoConnectionString());
            server = client.GetServer();
            var databaseName = MongoUrl.Create(MongoConnectionString()).DatabaseName;
            database = server.GetDatabase(databaseName);
            SiteAdminCollection = database.GetCollection<SiteAdmin>("SiteAdmin");
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