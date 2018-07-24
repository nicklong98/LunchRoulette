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
        public async Task CreatingALunchSpotWithANullCuisineThrowsCuisineNotFoundException()
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
            var retrievedLunchSpot = await services.GetLunchSpotByIdAsync(lunchSpot.Id);
            Assert.NotNull(retrievedLunchSpot);
            Assert.NotNull(retrievedLunchSpot.Cuisine);
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
            Assert.NotNull(createdLunchSpot.Cuisine);
        }

        [Theory]
        [InlineData("LunchSpotUpdateName", "Bob's Burgers", "Ted's Burgers")]
        public async Task ShouldBeAbleToUpdateNameOnALunchSpot(string dbName, string createName, string updatedName)
        {
            var cuisineServices = CreateCuisineServices(dbName);
            var services = CreateServices(dbName);
            var cuisine = await cuisineServices.CreateCuisineAsync(Guid.NewGuid().ToString());
            var createdLunchSpot = await services.CreateLunchSpotAsync(createName, cuisine);
            var updatedLunchSpot = await services.UpdateLunchSpotAsync(createdLunchSpot.Id, new LunchSpot(updatedName, cuisine));
            Assert.NotEqual(createdLunchSpot.Name, updatedLunchSpot.Name);
            Assert.Equal(createdLunchSpot.Id, updatedLunchSpot.Id);
            Assert.Equal(updatedName, updatedLunchSpot.Name);
        }

        [Theory]
        [InlineData("LunchSpotUpdateCuisine", "Italian", "French")]
        public async Task ShouldBeAbleToUpdateCuisineOnALunchSpot(string dbName, string createCuisine, string updatedCuisine)
        {
            var cuisineServices = CreateCuisineServices(dbName);
            var services = CreateServices(dbName);
            await cuisineServices.CreateCuisineAsync(createCuisine);
            var targetCuisine = await cuisineServices.CreateCuisineAsync(updatedCuisine);
            var createdLunchSpot = await services.CreateLunchSpotAsync(Guid.NewGuid().ToString(), new Cuisine(updatedCuisine));
            var updatedLunchSpot = await services.UpdateLunchSpotAsync(createdLunchSpot.Id, new LunchSpot(createdLunchSpot.Name, targetCuisine));
            Assert.Equal(createdLunchSpot.Id, updatedLunchSpot.Id);
            Assert.Equal(updatedLunchSpot.Cuisine.Id, targetCuisine.Id);
        }

        [Fact]
        public async Task UpdatingALunchSpotToANonExistantCuisineShouldThrowACuisineNotFoundException()
        {
            string dbName = Guid.NewGuid().ToString();
            var cuisineServices = CreateCuisineServices(dbName);
            var services = CreateServices(dbName);
            var cuisine = await cuisineServices.CreateCuisineAsync(Guid.NewGuid().ToString());
            var createdLunchSpot = await services.CreateLunchSpotAsync(Guid.NewGuid().ToString(), cuisine);
            await Assert.ThrowsAsync<CuisineNotFoundException>(
                () => services.UpdateLunchSpotAsync(createdLunchSpot.Id, 
                                                    new LunchSpot(createdLunchSpot.Name, new Cuisine(Guid.NewGuid().ToString()))));
        }

        [Fact]
        public async Task UpdatingANonExistantLunchSpotShouldThrowALunchSpotNotFoundException()
        {
            var services = CreateServices();
            await Assert.ThrowsAsync<LunchSpotNotFoundException>(() => services.UpdateLunchSpotAsync(-1, new LunchSpot()));
        }

        [Theory]
        [InlineData("FilteringByName", "Thai Flavor", "Thai")]
        [InlineData("FilteringByName", "Home", "American")]
        [InlineData("FilteringByName", "El Paso", "Mexican")]
        [InlineData("FilteringByName", "Subway", "Sandwich")]
        public async Task FilteringByNameShouldReturnOneResult(string dbName, string lunchSpotName, string cuisineName)
        {
            var cuisineServices = CreateCuisineServices(dbName);
            var services = CreateServices(dbName);
            var cuisine = await cuisineServices.CreateCuisineAsync(cuisineName);
            await services.CreateLunchSpotAsync(lunchSpotName, cuisine);
            var foundLunchSpot = services.ListLunchSpots(x => x.Name.EqualsIgnoreCase(lunchSpotName));
            Assert.All(foundLunchSpot.ToEnumerable(), x => Assert.NotNull(x.Cuisine));
            Assert.Equal(1, await services.ListLunchSpots(x => x.Name.EqualsIgnoreCase(lunchSpotName)).Count());
        }

        [Theory]
        [InlineData("FilteringByCuisine", "Thai Flavor", "Thai")]
        [InlineData("FilteringByCuisine", "Home", "American")]
        [InlineData("FilteringByCuisine", "El Paso", "Mexican")]
        [InlineData("FilteringByCuisine", "Subway", "Sandwich")]
        public async Task FilteringByCuisineShouldReturnOneResult(string dbName, string lunchSpotName, string cuisineName)
        {
            var cuisineServices = CreateCuisineServices(dbName);
            var services = CreateServices(dbName);
            await cuisineServices.CreateCuisineAsync(cuisineName);
            await services.CreateLunchSpotAsync(lunchSpotName, new Cuisine { Name = cuisineName });
            var filteredLunchSpots = services.ListLunchSpots(x => x.Cuisine.Name.EqualsIgnoreCase(cuisineName));
            Assert.Equal(1, await filteredLunchSpots.Count());
            Assert.All(filteredLunchSpots.ToEnumerable(), x => Assert.False(string.IsNullOrWhiteSpace(x.Name)));
        }
    }
}