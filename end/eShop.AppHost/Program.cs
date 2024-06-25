using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var insights = builder.AddAzureApplicationInsights("MyApplicationInsights");

// Databases

var postgres = builder.AddPostgres("postgres").WithPgAdmin();
var catalogDb = postgres.AddDatabase("catalogdb");

// Cache

var redis = builder.AddRedis("redis")
    .PublishAsAzureRedis();

// DB Manager Apps

builder.AddProject<Catalog_Data_Manager>("catalog-db-mgr")
    .WithReference(catalogDb);


// API Apps

var catalogApi = builder.AddProject<Catalog_API>("catalog-api")
    .WithReference(catalogDb)
    .WithReference(insights);

// Apps

builder.AddProject<WebApp>("webapp")
    .WithReference(catalogApi)
    .WithReference(redis)
    .WithReference(insights);


// Inject assigned URLs for Catalog API
catalogApi.WithEnvironment("CatalogOptions__PicBaseAddress", catalogApi.GetEndpoint("http"));

builder.Build().Run();
