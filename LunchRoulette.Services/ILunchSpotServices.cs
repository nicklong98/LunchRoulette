using System;
using System.Collections.Generic;

using LunchRoulette.Entities;

namespace LunchRoulette.Services
{
    public interface ILunchSpotServices
    {
        IAsyncEnumerable<LunchSpot> ListLunchSpots();
        IAsyncEnumerable<LunchSpot> ListLunchSpots(Func<LunchSpot, bool> filter);
    }
}
