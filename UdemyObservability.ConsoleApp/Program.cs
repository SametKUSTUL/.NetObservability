﻿// See https://aka.ms/new-console-template for more information
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using UdemyObservability.ConsoleApp;

Console.WriteLine("Hello, World!");



ActivitySource.AddActivityListener(new ActivityListener()
{
    ShouldListenTo = source => source.Name == OpenTelemetryConstants.ActivitySourceFileName,
    ActivityStarted = activity =>
    {
        Console.WriteLine("Activity Start");
    },
    ActivityStopped = activity =>
    {
        Console.WriteLine("Activity Stop");
    }
});


using var traceProvicerFile = Sdk.CreateTracerProviderBuilder()
    .AddSource(OpenTelemetryConstants.ActivitySourceFileName)
    .Build();



using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(OpenTelemetryConstants.ActivitySourceName)
    .ConfigureResource(configure =>
    {
        configure
        .AddService(OpenTelemetryConstants.ServiceName, serviceVersion: OpenTelemetryConstants.ServiceVersion, autoGenerateServiceInstanceId: true)
        .AddAttributes(new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("host.machineName", Environment.MachineName),
                    new KeyValuePair<string, object>("host.environment", "dev"),
                });
    }).AddConsoleExporter().AddOtlpExporter().AddZipkinExporter(zipkinOptions =>
    {
        zipkinOptions.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
    }).Build();

var serviceHelper=new ServiceHelper();
await serviceHelper.Work2();