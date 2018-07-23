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
        private ICuisineServices CreateCuisineServices(string dbName = null)
        {
            return new CuisineServices(DatabaseLayer.Context.LunchRouletteContextFactory.AsInMemory(dbName ?? Guid.NewGuid().ToString()));
        }

        private ILunchSpotServices CreateServices(string dbName = null)
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

        [Fact]
        public async Task CreatingALunchSpotWithAValidCuisineNameWorks()
        {
            string dbName = Guid.NewGuid().ToString();
            var cuisineServices = CreateCuisineServices(dbName);
            var services = CreateServices(dbName);
            var cuisine = await cuisineServices.CreateCuisineAsync(Guid.NewGuid().ToString());
            int initialCount = await services.ListLunchSpots().Count();
            await services.CreateLunchSpotAsync(Guid.NewGuid().ToString(), cuisine);
            Assert.Equal(initialCount + 1, await services.ListLunchSpots().Count());
        }

        [Fact]
        public async Task GettingANonExistantLunchSpotByIdThrowsLunchSpotNotFoundException()
        {
            var services = CreateServices();
            await Assert.ThrowsAsync<LunchSpotNotFoundException>(() => services.GetLunchSpotByIdAsync(-1));
        }

        [Fact]
        public async Task GettingALunchSpotByIdReturnsIfExists()
        {
            string dbName = Guid.NewGuid().ToString();
            var services = CreateServices(dbName);
            var cuisineServices = CreateCuisineServices(dbName);
            var cuisine = await cuisineServices.CreateCuisineAsync(Guid.NewGuid().ToString());
            var lunchSpot = await services.CreateLunchSpotAsync(Guid.NewGuid().ToString(), cuisine);
            Assert.NotNull(await services.GetLunchSpotByIdAsync(lunchSpot.Id));
        }

        [Theory]
        [InlineData("LunchSpotMixedCase", "bOB's lunCH SPOt", "Bob's Lunch Spot")]
        [InlineData("LunchSpotMixedCase", "joES AMERICaN Food", "Joes American Food")]
        [InlineData("LunchSpotMixedCase", "", "")]
        [InlineData("LunchSpotMixedCase", "Franks Bar", "Franks Bar")]
        [InlineData("LunchSpotMixedCase", "my cool spot", "My Cool Spot")]
        public async Task CreatingLunchSpotTransformsNameToTitleCase(string dbName, string createName, string expectedName)
        {
            var cuisineServices = CreateCuisineServices(dbName);
            var services = CreateServices(dbName);
            var cuisine = await cuisineServices.CreateCuisineAsync(Guid.NewGuid().ToString());
            var createdLunchSpot = await services.CreateLunchSpotAsync(createName, cuisine);
            Assert.Equal(expectedName, createdLunchSpot.Name);
        }
    }
}