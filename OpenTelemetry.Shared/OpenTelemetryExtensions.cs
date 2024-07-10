using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace OpenTelemetry.Shared
{
    public static class OpenTelemetryExtensions
    {
        public static void AddOpenTelemetryExt(this IServiceCollection services,IConfiguration configuration)
        {

            services.Configure<OpenTelemetryConstants>(configuration.GetSection("OpenTelemetry"));
            var openTelemetryConstants = (configuration.GetSection("OpenTelemetry").Get<OpenTelemetryConstants>())!;

            ActivitySourceProvider.Source = new System.Diagnostics.ActivitySource(openTelemetryConstants.ActivitySourceName, version: openTelemetryConstants.ServiceVersion);

            services.AddOpenTelemetry().WithTracing(options =>
            {
                options.AddSource(openTelemetryConstants.ActivitySourceName)
                .ConfigureResource(resource =>
                {
                    resource.AddService(openTelemetryConstants.ServiceName, serviceVersion: openTelemetryConstants.ServiceVersion);
                });

                options.AddAspNetCoreInstrumentation(aspNetCoreOptions =>
                {
                    aspNetCoreOptions.Filter = (context) =>
                    {
                        if (!string.IsNullOrEmpty(context.Request.Path.Value))
                        {
                            return context.Request.Path.Value.Contains("api", StringComparison.InvariantCulture);
                        }
                        return false;
                    };

                    aspNetCoreOptions.RecordException = true;// Hatanın detaylarıda kaydedilecek.
                    aspNetCoreOptions.EnrichWithException = (activity, exception) =>
                    {
                        activity.SetTag("EnrichExceptionStackTrace",exception.StackTrace);
                    };

                });

                options.AddEntityFrameworkCoreInstrumentation(efCoreOptions =>
                {
                    efCoreOptions.SetDbStatementForText=true;
                    efCoreOptions.SetDbStatementForStoredProcedure=true;
                    efCoreOptions.EnrichWithIDbCommand = (activity, dbCommand) =>
                    {
                        activity.SetTag("db.connection.string", dbCommand.Connection?.ConnectionString);
                    };
                });

                options.AddHttpClientInstrumentation(httpClientOptions =>
                {
                    httpClientOptions.EnrichWithHttpRequestMessage = async (activity, request) =>
                    {
                        var requestContent=string.Empty;
                        if (request.Content != null)
                        {
                            requestContent=await request.Content.ReadAsStringAsync();
                        }
                        activity.SetTag("http.request.body", requestContent);
                    };
                    httpClientOptions.EnrichWithHttpResponseMessage = async (activity, response) =>
                    {
                        var responseContent=string.Empty;
                        if (response.Content != null)
                        {
                            responseContent=await response.Content.ReadAsStringAsync();
                        }
                        activity.SetTag("http.response.body", responseContent);
                    };
                });
                //options.AddConsoleExporter(); // Konsola export et
                options.AddOtlpExporter(); // Jaeger a export et
            });
        }
    }
}
