using System;
using System.Linq;
using System.Collections.Generic;
using LunchRoulette.DatabaseLayer.Context;
using LunchRoulette.Entities;

namespace LunchRoulette.Services
{
    public class LunchSpotServices : ILunchSpotServices
    {
        private LunchRouletteContext _context { get; }

        public LunchSpotServices(LunchRouletteContext context)
        {
            _context = context;
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