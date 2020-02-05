﻿using System.Diagnostics.CodeAnalysis;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NHSD.BuyingCatalogue.Documents.API.Config;
using NHSD.BuyingCatalogue.Documents.API.HealthChecks;
using NHSD.BuyingCatalogue.Documents.API.Repositories;

namespace NHSD.BuyingCatalogue.Documents.API
{
    [SuppressMessage("Design", "CA1822", Justification = "ASP.Net needs this to not be static")]
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var settings = Configuration.GetSection("AzureBlobStorage").Get<AzureBlobStorageSettings>();
            services.AddSingleton<IAzureBlobStorageSettings>(settings);

            services.AddTransient(x =>
            {
                var settings = x.GetService<IAzureBlobStorageSettings>();
                return new BlobServiceClient(settings.ConnectionString)
                    .GetBlobContainerClient(settings.ContainerName);
            });

            services.AddCustomHealthChecks(settings);

            services.AddTransient<IDocumentRepository, AzureBlobDocumentRepository>();
            services.AddControllers();
            services.AddSwaggerGen(options =>
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Documents API",
                    Version = "v1",
                    Description = "NHS Digital GP IT Buying Catalogue Documents HTTP API"
                }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage()
                    .UseSwagger()
                    .UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Buying Catalogue Documents API V1");
                    });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/live",
                    new HealthCheckOptions
                    {
                        Predicate = healthCheckRegistration =>
                            healthCheckRegistration.Tags.Contains(HealthCheckTags.Live)
                    });

                endpoints.MapHealthChecks("/health/ready",
                    new HealthCheckOptions
                    {
                        Predicate = healthCheckRegistration =>
                            healthCheckRegistration.Tags.Contains(HealthCheckTags.Ready)
                    });
            });

            app.UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
