using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace VirtoCommerce.Storefront.Telemetry
{
    public class TelemetryHeadersInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TelemetryHeadersInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var requestTelemetry = telemetry as RequestTelemetry;
            // Is this a TrackRequest() ?
            if (requestTelemetry == null)
            {
                return;
            }
            
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                return;
            }

            foreach (var header in context.Request.Headers)
            {
                telemetry.Context.Properties.Add($"Request-{header.Key}", string.Join(Environment.NewLine, header));
            }
            foreach (var header in context.Response.Headers)
            {
                telemetry.Context.Properties.Add($"Response-{header.Key}", string.Join(Environment.NewLine, header));
            }
        }
    }
}
