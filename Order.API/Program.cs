using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Order.API.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<OpenTelemetryConstants>(builder.Configuration.GetSection("OpenTelemetry"));
var openTelemetryConstants = (builder.Configuration.GetSection("OpenTelemetry").Get<OpenTelemetryConstants>())!;

builder.Services.AddOpenTelemetry().WithTracing(options =>
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
        
    }); 
    //options.AddConsoleExporter(); // Konsola export et
    options.AddOtlpExporter(); // Jaeger a export et
});


ActivitySourceProvider.Source = new System.Diagnostics.ActivitySource(openTelemetryConstants.ActivitySourceName,version:openTelemetryConstants.ServiceVersion);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
