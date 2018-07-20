using System;

namespace LunchRoulette.Entities
{
    public class Cuisine
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Cuisine() { }

        public Cuisine(LunchRoulette.DatabaseLayer.Entities.Cuisine cuisine)
        {
            Id = cuisine.Id;
            Name = cuisine.Name;
        }
    }

    public class LunchSpot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Cuisine Cuisine { get; set; }

        public LunchSpot() { }

        public LunchSpot(LunchRoulette.DatabaseLayer.Entities.LunchSpot lunchSpot)
        {
            Id = lunchSpot.Id;
            Name = lunchSpot.Name;
            Cuisine = lunchSpot.Cuisine != null ? new Cuisine(lunchSpot.Cuisine) : null;
        }
    }
}
