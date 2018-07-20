using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using LunchRoulette.DatabaseLayer.Context;
using LunchRoulette.Entities;
using Microsoft.EntityFrameworkCore;
using LunchRoulette.Exceptions.CuisineExceptions;
using LunchRoulette.Utils.StringHelpers;
using LunchRoulette.Utils.IQueryableHelpers;

namespace LunchRoulette.Services
{
    public class CuisineServices : ICuisineServices
    {
        private LunchRouletteContext _context { get; }

        public CuisineServices(LunchRouletteContext context)
        {
            _context = context;
        }

        public async Task<Cuisine> CreateCuisineAsync(string cuisineName)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var cuisine = await (from x in _context.Cuisines
                                     where x.Name.EqualsIgnoreCase(cuisineName)
                                     select x).SingleOrDefaultAsync();
                if (cuisine == null)
                {
                    cuisine = new DatabaseLayer.Entities.Cuisine(cuisineName.ToTitleCase());
                    await _context.AddAsync(cuisine);
                    await _context.SaveChangesAsync();
                }
                _context.Database.CommitTransaction();
                return new Cuisine(cuisine);
            }
            catch (Exception)
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }

        public async Task<Cuisine> GetCuisineByIdAsync(int cuisineId)
        {
            return await (from x in _context.Cuisines where x.Id == cuisineId select new Cuisine(x))
                .Extend()
                .SingleOrThrowAsync<CuisineNotFoundException>();
        }

        public IAsyncEnumerable<Cuisine> ListCuisines()
        {
            return (from x in _context.Cuisines select new Cuisine(x)).ToAsyncEnumerable();
        }

        public IAsyncEnumerable<Cuisine> ListCuisines(Func<Cuisine, bool> cuisineFilter)
        {
            return (from x in _context.Cuisines select new Cuisine(x)).Where(cuisineFilter).ToAsyncEnumerable();
        }

        public async Task<Cuisine> UpdateCuisineAsync(int cuisineId, Cuisine cuisine)
        {
            var targetCuisine = await (from x in _context.Cuisines where x.Id == cuisineId select x).Extend().SingleOrThrowAsync<CuisineNotFoundException>();
            try
            {
                await _context.Database.BeginTransactionAsync();
                if (await _context.Cuisines.AnyAsync(x => x.Name.EqualsIgnoreCase(cuisine.Name))) throw new CuisineException();
                targetCuisine.Name = cuisine.Name;
                _context.Entry(targetCuisine).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();
                return new Cuisine(targetCuisine);
            }
            catch (Exception)
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }
    }
}