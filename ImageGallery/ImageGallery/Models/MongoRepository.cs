﻿using System;
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
    public class MongoRepository
    {
        MongoServer server = null;
        MongoDatabase database = null;
        MongoCollection WidgetCollection = null;

        public MongoRepository()
        {
            Init();
        }

        public MongoCollection Widget
        {
            get
            {
                return WidgetCollection;
            }
        }

        private void Init()
        {
            server = MongoServer.Create(MongoConnectionString());
            var databaseName = MongoUrl.Create(MongoConnectionString()).DatabaseName;
            database = server.GetDatabase(databaseName);
            WidgetCollection = database.GetCollection<Widget>("Widget");
        }

        public string MongoConnectionString()
        {
            var r = ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
                ConfigurationManager.AppSettings.Get("MONGOLAB_URI") ??
                "mongodb://192.168.1.101/Things";
            return r;
        }

    }

    public class Widget
    {
        public ObjectId id { get; set; }
        public string Name { get; set; }
    }
}