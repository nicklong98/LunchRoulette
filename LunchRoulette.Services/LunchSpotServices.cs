using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using LunchRoulette.DatabaseLayer.Context;
using LunchRoulette.Entities;
using LunchRoulette.Utils.StringHelpers;
using LunchRoulette.Utils.IQueryableHelpers;
using LunchRoulette.Exceptions.CuisineExceptions;

namespace LunchRoulette.Services
{
    public class LunchSpotServices : ILunchSpotServices
    {
        private LunchRouletteContext _context { get; }
        private ICuisineServices _cuisineServices{ get; }

        public LunchSpotServices(ICuisineServices cuisineServices, LunchRouletteContext context)
        {
            _cuisineServices = cuisineServices;
            _context = context;
        }

        public async Task<LunchSpot> CreateLunchSpotAsync(string lunchSpotName, Cuisine cuisine)
        {
            var lunchSpot = new LunchRoulette.DatabaseLayer.Entities.LunchSpot
            {
                Name = lunchSpotName,
                CuisineId = await _cuisineServices.ListCuisines(x=>x.Name.EqualsIgnoreCase(cuisine?.Name)).Select(x=>x.Id)
                    .Extend()
                    .SingleOrThrowAsync<CuisineNotFoundException>()
            };
            await _context.AddAsync(lunchSpot);
            await _context.SaveChangesAsync();
            return new LunchSpot(lunchSpot);
        }

        public IAsyncEnumerable<LunchSpot> ListLunchSpots()
        {
            return (from x in _context.LunchSpots select new LunchSpot(x)).ToAsyncEnumerable();
        }

        public IAsyncEnumerable<LunchSpot> ListLunchSpots(Func<LunchSpot, bool> filter)
        {
            return (from x in _context.LunchSpots select new LunchSpot(x)).Where(filter).ToAsyncEnumerable();
        }
    }
}