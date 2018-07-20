using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using LunchRoulette.DatabaseLayer.Entities;

namespace LunchRoulette.DatabaseLayer.Context
{
    public class LunchRouletteContext : DbContext
    {
        public DbSet<LunchSpot> LunchSpots { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }

        public LunchRouletteContext(DbContextOptions<LunchRouletteContext> options) : base(options) { }
    }

    public class LunchRouletteContextFactory : IDesignTimeDbContextFactory<LunchRouletteContext>
    {
        public static LunchRouletteContext AsInMemory(string dbName)
        {
            var builder = new DbContextOptionsBuilder<LunchRouletteContext>();
            builder.UseInMemoryDatabase(dbName);
            return new LunchRouletteContext(builder.Options);
        }

        public static LunchRouletteContext AsPostgresql(string connectionString)
        {
            var builder = new DbContextOptionsBuilder<LunchRouletteContext>();
            builder.UseNpgsql(connectionString);
            return new LunchRouletteContext(builder.Options);
        }

        public LunchRouletteContext CreateDbContext(string[] args)
        {
            return LunchRouletteContextFactory.AsInMemory(args.FirstOrDefault());
        }
    }
}