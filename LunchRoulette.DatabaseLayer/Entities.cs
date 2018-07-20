using System;

namespace LunchRoulette.DatabaseLayer.Entities
{
    public class Cuisine
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Cuisine() { }
        public Cuisine(string name)
        {
            Name = name;
        }
    }

    public class LunchSpot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CuisineId { get; set; }
        public virtual Cuisine Cuisine { get; set; }

        public LunchSpot() { }

        public LunchSpot(string name, Cuisine cuisine)
        {
            Name = name;
            Cuisine = cuisine;
        }
    }
}
