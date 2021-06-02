using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;

namespace VirtoCommerce.Storefront.Infrastructure.ApplicationInsights
{
    public class DemoUserTelemetryInitializer : UserTelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DemoUserTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnInitializeTelemetry(HttpContext platformContext, RequestTelemetry requestTelemetry, ITelemetry telemetry)
        {
            base.OnInitializeTelemetry(platformContext, requestTelemetry, telemetry);

            var httpContext = _httpContextAccessor.HttpContext;

            const string healthCheckHeader = "Health-Check";

            if (httpContext.Request.Headers.ContainsKey(healthCheckHeader))
            {
                requestTelemetry.Context.GlobalProperties[healthCheckHeader] = "true";
            }
        }
    }
}
