using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Musala.Drones.Domain;
using Musala.Drones.Domain.ServicesContracts;
using Musala.Drones.MongoInfrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Musala.Drones.ApiHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private string mongoDBConfigHeader = "MongoDB";
        private string telemetryPeriodHeader = "TelemetryPeriod";
        public MongoDbConfiguration MongoDbConfiguration
        {
            get
            {
                var temp = Configuration.GetSection(mongoDBConfigHeader).Get<MongoDbConfiguration>();
                return temp;
            }
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDbServiceHealth>((sp) =>
            {
                return new MongoDbServiceHealth(MongoDbConfiguration);
            });
            services.AddSingleton<IMongoClient>((sp) =>
            {
                var mongoUrl = MongoUrl.Create(MongoDbConfiguration.ConnectionString);
                MongoClientSettings mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
                var client = new MongoClient(mongoClientSettings);
                var _ = client.GetDatabase(MongoDbConfiguration.DatabaseName);
                return client;
            });
            services.AddSingleton<DroneStorageService>((sp) =>
            {
                var client = sp.GetService<IMongoClient>();
                var service = new DroneStorageService(client, MongoDbConfiguration);
                return service;
            });
            services.AddSingleton<IDronesStorageServices>((sp) =>
            {
                return sp.GetService<DroneStorageService>();
            });
            services.AddSingleton<IDbCleaner>((sp) =>
            {
                return sp.GetService<DroneStorageService>();
            });
            services.AddSingleton<IDroneTelemetryService>((sp) =>
            {
                var telemetryPeriod = Configuration.GetSection(telemetryPeriodHeader).Get<int>();
                return new DroneTelemetryServiceMock(telemetryPeriod*1000);
            });
            services.AddSingleton<IDroneServices>((sp) =>
            {
                var storage = sp.GetService<DroneStorageService>();
                var telemetry = sp.GetService<IDroneTelemetryService>();
                var telemetryPeriod = Configuration.GetSection(telemetryPeriodHeader).Get<int>();
                var service = new DroneServices(storage, telemetry, telemetryPeriod * 1000);
                return service;
            });
            services.AddControllers();
            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Musala.Drones.ApiHost", Version = "v1" });


                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/api/health/WebHost");
            //if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Musala.Drones.ApiHost v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
