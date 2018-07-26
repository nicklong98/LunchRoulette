using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace LunchRoulette.Web
{
    public class Startup
    {

        private enum DatabaseType
        {
            Postgres,
            InMemory
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void InitDb(DatabaseType dbType, string connectionString)
        {
            switch (dbType)
            {
                case DatabaseType.Postgres:
                    using (var context = LunchRoulette.DatabaseLayer.Context.LunchRouletteContextFactory.AsPostgresql(connectionString))
                        context.Database.Migrate();
                    break;
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            DatabaseType dbType = System.Enum.Parse<DatabaseType>(Configuration.GetValue<string>("Data:DatabaseType"));
            string connectionString = Configuration.GetValue<string>("Data:ConnectionString");
            InitDb(dbType, connectionString);

            services.AddDbContext<LunchRoulette.DatabaseLayer.Context.LunchRouletteContext>(optionsBuilder =>
            {
                switch (dbType)
                {
                    case DatabaseType.InMemory:
                        optionsBuilder.UseInMemoryDatabase(connectionString);
                        optionsBuilder.ConfigureWarnings(warnings =>
                            warnings.Default(WarningBehavior.Log)
                                    .Ignore(InMemoryEventId.TransactionIgnoredWarning)
                        );
                        break;
                    case DatabaseType.Postgres:
                        optionsBuilder.UseNpgsql(connectionString);
                        break;
                }
            });

            services.AddTransient<LunchRoulette.Services.ICuisineServices, LunchRoulette.Services.CuisineServices>();
            services.AddTransient<LunchRoulette.Services.ILunchSpotServices, LunchRoulette.Services.LunchSpotServices>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            services.AddSingleton(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
