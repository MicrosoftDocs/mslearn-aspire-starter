using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client.Extensions.Msal;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");

if (builder.Environment.IsDevelopment())
{
    storage.RunAsEmulator();
}

var queues = storage.AddQueues("queueConnection");

// Databases

var postgres = builder.AddPostgres("postgres").WithPgAdmin();
var catalogDb = postgres.AddDatabase("CatalogDB");

// DB Manager Apps

builder.AddProject<Catalog_Data_Manager>("catalog-db-mgr")
    .WithReference(catalogDb);


// API Apps

var catalogApi = builder.AddProject<Catalog_API>("catalog-api")
    .WithReference(catalogDb)
    .WithReference(queues);

// Apps

builder.AddProject<WebApp>("webapp")
    .WithReference(catalogApi);

// Inject assigned URLs for Catalog API
catalogApi.WithEnvironment("CatalogOptions__PicBaseAddress", () => catalogApi.GetEndpoint("http").Url);

builder.AddProject<Projects.eShop_MessageProcessor>("eshop-messageprocessor")
    .WithReference(queues);

builder.Build().Run();
