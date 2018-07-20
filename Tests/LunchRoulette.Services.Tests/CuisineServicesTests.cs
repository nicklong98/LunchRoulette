using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchRoulette.Entities;
using LunchRoulette.Exceptions.CuisineExceptions;
using LunchRoulette.Utils.StringHelpers;
using Xunit;

namespace LunchRoulette.Services.Tests
{
    public class CuisineServicesTests
    {
        private Services.ICuisineServices CreateServices(string connectionString = null)
        {
            return new Services.CuisineServices(
                DatabaseLayer.Context.LunchRouletteContextFactory.AsInMemory(connectionString ?? Guid.NewGuid().ToString()));
        }

        [Fact]
        public async Task CreatingANewCuisineResultsInOneNewCuisine()
        {
            var services = CreateServices();
            int currentCount = await services.ListCuisines().Count();
            var createdCuisine = await services.CreateCuisineAsync(Guid.NewGuid().ToString());
            int nextCount = await services.ListCuisines().Count();
            Assert.Equal(currentCount + 1, nextCount);
        }

        [Fact]
        public async Task CreatingCuisineTranslatesToTitleCase()
        {
            string cuisineName = "bOB's cuiSINE TyPE";
            var services = CreateServices();
            var cuisine = await services.CreateCuisineAsync(cuisineName);
            Assert.Equal(cuisineName.ToTitleCase(), cuisine.Name);
        }

        [Fact]
        public async Task CreatingWithDuplicateNameReturnsCurrentEntry()
        {
            var services = CreateServices();
            var firstCuisine = await services.CreateCuisineAsync(Guid.NewGuid().ToString());
            var duplicateCuisine = await services.CreateCuisineAsync(firstCuisine.Name);
            Assert.Equal(firstCuisine.Id, duplicateCuisine.Id);
        }

        [Fact]
        public async Task CreatingCuisineIgnoresCaseWhenCheckingForDuplicate()
        {
            var services = CreateServices();
            var firstCuisine = await services.CreateCuisineAsync(Guid.NewGuid().ToString().ToLower());
            var duplicateCuisine = await services.CreateCuisineAsync(firstCuisine.Name.ToUpper());
            Assert.Equal(firstCuisine.Id, duplicateCuisine.Id);
        }

        [Fact]
        public async Task ListingCuisinesShouldReturnAllCuisines()
        {
            var services = CreateServices();
            for (int i = 0; i < 5;i++)
                await services.CreateCuisineAsync(Guid.NewGuid().ToString());
            Assert.Equal(5, await services.ListCuisines().Count());
        }

        [Fact]
        public async Task ShouldBeAbleToGetCuisineById()
        {
            var services = CreateServices();
            var cuisines = new List<Cuisine>();
            for (int i = 0; i < 5;i++)
                cuisines.Add(await services.CreateCuisineAsync(Guid.NewGuid().ToString()));
            var targetCuisine = cuisines[2];
            Assert.Equal(targetCuisine.Id, (await services.GetCuisineByIdAsync(targetCuisine.Id)).Id);
        }

        [Fact]
        public async Task RequestingAnInvalidCuisineByIdShouldThrowCuisineNotFoundException()
        {
            var services = CreateServices();
            await Assert.ThrowsAsync<CuisineNotFoundException>(() => services.GetCuisineByIdAsync(1));
        }

        [Theory]
        [InlineData("FilterTest", "thai")]
        [InlineData("FilterTest", "italian")]
        [InlineData("FilterTest", "chinese")]
        [InlineData("FilterTest", "american")]
        [InlineData("FilterTest", "indian")]
        public async Task FilteringByNameShouldReturnOneCuisine(string dbName, string cuisineName)
        {
            var services = CreateServices(dbName);
            var cuisine = await services.CreateCuisineAsync(cuisineName);
            Assert.Equal(1, await services.ListCuisines((c => c.Name.EqualsIgnoreCase(cuisineName))).Count());
        }

        [Fact]
        public async Task FilteringByNonExistantNameShouldReturnEmptyIAsyncEnumerable()
        {
            var services = CreateServices();
            Assert.True(await services.ListCuisines((x => x.Name == Guid.NewGuid().ToString())).IsEmpty());
        }

        [Fact]
        public async Task UpdatingACuisineShouldChangeIt()
        {
            var services = CreateServices();
            var origCuisine = await services.CreateCuisineAsync(Guid.NewGuid().ToString());
            var cuisineChanges = new Cuisine { Name = Guid.NewGuid().ToString() };
            var updatedCuisine = await services.UpdateCuisineAsync(origCuisine.Id, cuisineChanges);
            Assert.NotEqual(origCuisine.Name, updatedCuisine.Name);
        }

        [Fact]
        public async Task UpdatingANonExistantCuisineShouldThrowACuisineNotFoundException()
        {
            var services = CreateServices();
            await Assert.ThrowsAsync<CuisineNotFoundException>(() => services.UpdateCuisineAsync(-1, new Cuisine()));
        }

        [Fact]
        public async Task UpdatingACuisineToADuplicateNameIsNotAllowed()
        {
            var services = CreateServices();
            var origCuisine = await services.CreateCuisineAsync(Guid.NewGuid().ToString());
            var cuisineToUpdate = await services.CreateCuisineAsync(Guid.NewGuid().ToString());
            await Assert.ThrowsAsync<CuisineException>(() => 
                services.UpdateCuisineAsync(cuisineToUpdate.Id, new Cuisine { Name = origCuisine.Name }));
        }
    }
}
