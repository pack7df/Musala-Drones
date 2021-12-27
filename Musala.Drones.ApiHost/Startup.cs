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
using System.Linq;
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
                return Configuration.GetSection(mongoDBConfigHeader).Get<MongoDbConfiguration>();
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
                return new DroneStorageService(client, MongoDbConfiguration);
            });
            services.AddSingleton<IDronesStorageServices>((sp) =>
            {
                return sp.GetService<DroneStorageService>();
            });
            services.AddSingleton<IDbCleaner>((sp) =>
            {
                return sp.GetService<DroneStorageService>();
            });
            services.AddSingleton<IDroneTelemetryService,DroneTelemetryServiceMock>();
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
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/api/health/WebHost");
            if (env.IsDevelopment())
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
