using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LunchRoulette.Entities;

namespace LunchRoulette.Services
{
    public interface ILunchSpotServices
    {
        Task<LunchSpot> CreateLunchSpotAsync(string lunchSpotName, Cuisine cuisine);
        Task<LunchSpot> GetLunchSpotByIdAsync(int lunchSpotId);

        IAsyncEnumerable<LunchSpot> ListLunchSpots();
        IAsyncEnumerable<LunchSpot> ListLunchSpots(Func<LunchSpot, bool> filter);
    }
}
