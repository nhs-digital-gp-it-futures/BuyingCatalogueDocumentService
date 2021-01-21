using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NHSD.BuyingCatalogue.Documents.API.Config;
using NHSD.BuyingCatalogue.Documents.API.HealthChecks;
using NHSD.BuyingCatalogue.Documents.API.Logging;
using NHSD.BuyingCatalogue.Documents.API.Repositories;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NHSD.BuyingCatalogue.Documents.API
{
    public sealed class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var settings = Configuration.GetSection("AzureBlobStorage").Get<AzureBlobStorageSettings>();
            services.AddSingleton<IAzureBlobStorageSettings>(settings);

            services.AddTransient(_ => AzureBlobContainerClientFactory.Create(settings));

            services.AddCustomHealthChecks(settings);

            services.AddTransient<IDocumentRepository, AzureBlobDocumentRepository>();
            services.AddControllers();
            services.AddSwaggerGen(options =>
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Documents API",
                        Version = "v1",
                        Description = "NHS Digital GP IT Buying Catalogue Documents HTTP API",
                    }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Invoked by runtime.")]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage()
                    .UseSwagger()
                    .UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Buying Catalogue Documents API V1");
                    });
            }

            app.UseSerilogRequestLogging(o => o.GetLevel = SerilogRequestLoggingOptions.GetLevel);

            app.UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();

                    endpoints.MapHealthChecks("/health/live",
                        new HealthCheckOptions
                        {
                            Predicate = healthCheckRegistration =>
                                healthCheckRegistration.Tags.Contains(HealthCheckTags.Live),
                        });

                    endpoints.MapHealthChecks("/health/ready",
                        new HealthCheckOptions
                        {
                            Predicate = healthCheckRegistration =>
                                healthCheckRegistration.Tags.Contains(HealthCheckTags.Ready),
                        });
                });

            LogConfiguration(app.ApplicationServices, logger);
        }

        private static void LogConfiguration(IServiceProvider serviceProvider, ILogger logger)
        {
            var settings = serviceProvider.GetRequiredService<IAzureBlobStorageSettings>();

            logger.LogInformation("Configuration:\n{0}", settings);
        }
    }
}
