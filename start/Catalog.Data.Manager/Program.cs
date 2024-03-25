using eShop.Catalog.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("sqlconnection") ?? throw new InvalidOperationException("Connection string 'sqlconnection' not found.")));

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
  // Retrieve an instance of the DbContext class and manually run migrations during startup
  using (var scope = app.Services.CreateScope())
  {
    var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    string script = File.ReadAllText(@"Setup\backup.sql");
    var rowsChanged = context.Database.ExecuteSqlRaw(script);
  }
}

app.Run();
