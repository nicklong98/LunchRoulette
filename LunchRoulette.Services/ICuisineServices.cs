using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LunchRoulette.Entities;

namespace LunchRoulette.Services
{
    public interface ICuisineServices
    {
        Task<Cuisine> CreateCuisineAsync(string cuisineName);
        Task<Cuisine> GetCuisineByIdAsync(int cuisineId);
        Task<Cuisine> UpdateCuisineAsync(int cuisineId, Cuisine cuisine);

        IAsyncEnumerable<Cuisine> ListCuisines();
        IAsyncEnumerable<Cuisine> ListCuisines(Func<Cuisine, bool> cuisineFilter);
    }
}