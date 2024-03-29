﻿using System;
using System.ComponentModel.DataAnnotations;

namespace LunchRoulette.Entities
{
    public class Cuisine
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Cuisine() { }

        public Cuisine(string cuisineName)
        {
            Name = cuisineName;
        }

        public Cuisine(LunchRoulette.DatabaseLayer.Entities.Cuisine cuisine)
        {
            if(cuisine == null) return;
            Id = cuisine.Id;
            Name = cuisine.Name;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public class LunchSpot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Cuisine Cuisine { get; set; }

        public LunchSpot() { }

        public LunchSpot(string lunchSpotName, Cuisine cuisine)
        {
            Name = lunchSpotName;
            Cuisine = cuisine;
        }

        public LunchSpot(LunchRoulette.DatabaseLayer.Entities.LunchSpot lunchSpot)
        {
            Id = lunchSpot.Id;
            Name = lunchSpot.Name;
            Cuisine = lunchSpot.Cuisine != null ? new Cuisine(lunchSpot.Cuisine) : null;
        }
    }
}
