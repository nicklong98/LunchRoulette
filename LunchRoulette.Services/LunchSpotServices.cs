using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using LunchRoulette.DatabaseLayer.Context;
using LunchRoulette.Entities;
using LunchRoulette.Utils.StringHelpers;
using LunchRoulette.Utils.IQueryableHelpers;
using LunchRoulette.Exceptions.CuisineExceptions;
using LunchRoulette.Exceptions.LunchSpotExceptions;
using Microsoft.EntityFrameworkCore;

namespace LunchRoulette.Services
{
    public class LunchSpotServices : ILunchSpotServices
    {
        private LunchRouletteContext _context { get; }
        private ICuisineServices _cuisineServices { get; }

        public LunchSpotServices(ICuisineServices cuisineServices, LunchRouletteContext context)
        {
            _cuisineServices = cuisineServices;
            _context = context;
        }

        public async Task<LunchSpot> CreateLunchSpotAsync(string lunchSpotName, Cuisine cuisine)
        {
            var targetCuisine = await _cuisineServices.ListCuisines(x => x.Name.EqualsIgnoreCase(cuisine?.Name))
                                                                    .Extend()
                                                                    .SingleOrThrowAsync<CuisineNotFoundException>();
            var lunchSpot = new LunchRoulette.DatabaseLayer.Entities.LunchSpot
            {
                Name = lunchSpotName.ToTitleCase(),
                CuisineId = targetCuisine.Id
            };
            await _context.AddAsync(lunchSpot);
            await _context.SaveChangesAsync();
            return new LunchSpot(lunchSpot) { Cuisine = targetCuisine };
        }

        public async Task<LunchSpot> GetLunchSpotByIdAsync(int lunchSpotId)
        {
            return await (from x in _context.LunchSpots where x.Id == lunchSpotId select x)
                            .Include(x => x.Cuisine)
                            .Select(x => new LunchSpot(x))
                            .Extend()
                            .SingleOrThrowAsync<LunchSpotNotFoundException>();
        }

        public async Task<LunchSpot> UpdateLunchSpotAsync(int lunchSpotId, LunchSpot lunchSpot)
        {
            var targetLunchSpot = await (from x in _context.LunchSpots where x.Id == lunchSpotId select x)
                                        .Extend()
                                        .SingleOrThrowAsync<LunchSpotNotFoundException>();
            targetLunchSpot.Name = lunchSpot.Name.ToTitleCase();
            var targetCuisine = await _cuisineServices.ListCuisines(x => x.Name.EqualsIgnoreCase(lunchSpot.Cuisine?.Name))
                                .Extend()
                                .SingleOrThrowAsync<CuisineNotFoundException>();
            targetLunchSpot.CuisineId = targetCuisine.Id;
            _context.Entry(targetLunchSpot).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new LunchSpot(targetLunchSpot) { Cuisine = targetCuisine };
        }

        public IAsyncEnumerable<LunchSpot> ListLunchSpots()
        {
            return (from x in _context.LunchSpots select x)
                    .Include(x => x.Cuisine)
                    .Select(x => new LunchSpot(x))
                    .ToAsyncEnumerable();
        }

        public IAsyncEnumerable<LunchSpot> ListLunchSpots(Func<LunchSpot, bool> filter)
        {
            return (from x in _context.LunchSpots select x)
                    .Include(x => x.Cuisine)
                    .Select(x => new LunchSpot(x))
                    .Where(filter).ToAsyncEnumerable();
        }
    }
}