﻿using System;
using Microsoft.AspNetCore.Http;
using Serilog.Events;

namespace NHSD.BuyingCatalogue.Documents.API.Logging
{
    internal static class SerilogRequestLoggingOptions
    {
        internal const string HealthCheckEndpointDisplayName = "Health checks";

        internal static LogEventLevel GetLevel(HttpContext? httpContext, double elapsed, Exception? exception)
        {
            _ = elapsed;

            if (exception is not null)
                return LogEventLevel.Error;

            if (httpContext is null || httpContext.Response.StatusCode > 499)
                return LogEventLevel.Error;

            return IsHealthCheck(httpContext)
                ? LogEventLevel.Verbose
                : LogEventLevel.Information;
        }

        private static bool IsHealthCheck(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();

            return endpoint is not null && string.Equals(
               endpoint.DisplayName,
               HealthCheckEndpointDisplayName,
               StringComparison.OrdinalIgnoreCase);
        }
    }
}
