using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchRoulette.Entities;
using LunchRoulette.Exceptions.LunchSpotExceptions;
using LunchRoulette.Exceptions.CuisineExceptions;
using LunchRoulette.Utils.StringHelpers;
using Xunit;

namespace LunchRoulette.Services.Tests
{
    public class LunchSpotServicesTests
    {
        public ILunchSpotServices CreateServices(string dbName = null)
        {
            dbName = dbName ?? Guid.NewGuid().ToString();
            var context = DatabaseLayer.Context.LunchRouletteContextFactory.AsInMemory(dbName);
            return new LunchSpotServices(new CuisineServices(context), context);
        }

        [Fact]
        public async Task CreatingALunchSpotWithANullCuisineIsAllowed()
        {
            var services = CreateServices();
            await Assert.ThrowsAsync<CuisineNotFoundException>(() =>
                services.CreateLunchSpotAsync(Guid.NewGuid().ToString(), null));
        }

        [Fact]
        public async Task CreatingALunchSpotWithANonExistantCuisineThrowsACuisineNotFoundException()
        {
            var services = CreateServices();
            await Assert.ThrowsAsync<CuisineNotFoundException>(() => 
                services.CreateLunchSpotAsync(Guid.NewGuid().ToString(), new Cuisine { Name = Guid.NewGuid().ToString() }));
        }
    }
}