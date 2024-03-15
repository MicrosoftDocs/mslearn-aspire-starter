using eShop.Catalog.API;
using eShop.Catalog.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDefaultOpenApi();
builder.AddApplicationServices();

builder.Services.AddProblemDetails();

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("sqlconnection") ?? throw new InvalidOperationException("Connection string 'sqlconnection' not found.")));


var app = builder.Build();

app.UseDefaultOpenApi();

app.MapDefaultEndpoints();

app.MapGroup(app.GetOptions<CatalogOptions>().ApiBasePath)
    .WithTags("Catalog API")
    .MapCatalogApi();

app.Run();
