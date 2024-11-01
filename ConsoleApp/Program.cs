﻿// See https://aka.ms/new-console-template for more information

using ConsoleApp;
using ConsoleApp.Models.Configuration;
using ConsoleApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<WorkerService>();
        services.AddOptions<ConsoleAppSettings>().BindConfiguration(nameof(ConsoleAppSettings));
        services.AddScoped<IDemoService, DemoService>();
    })
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        configBuilder.AddUserSecrets<Program>();
    })
    .UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

using var host = builder.Build();

await host.RunAsync();
