using Microsoft.Extensions.Hosting;
using Review.API;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDefaultOpenApi();

builder.Services.AddProblemDetails();

builder.AddCosmosDbContext<ReviewDbContext>("cdb", "cosmosdb");

var app = builder.Build();

app.UseDefaultOpenApi();

app.MapDefaultEndpoints();

app.Run();