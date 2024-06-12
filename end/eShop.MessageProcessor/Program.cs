using eShop.MessageProcessor;

var builder = Host.CreateApplicationBuilder(args);

builder.AddAzureQueueClient("queueConnection");

builder.AddServiceDefaults();
builder.Services.AddHostedService<WorkerService>();

var host = builder.Build();
host.Run();
