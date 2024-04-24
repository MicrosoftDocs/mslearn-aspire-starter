using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitConsumer;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddRabbitMQ("messaging");

builder.Services.AddHostedService<CatalogProcessingJob>();

var host = builder.Build();

host.Run();