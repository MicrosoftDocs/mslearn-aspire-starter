using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// RabbitMQ message broker

var messaging = builder.AddRabbitMQContainer("messaging");

// Databases

var postgres = builder.AddPostgres("postgres").WithPgAdmin();
var catalogDb = postgres.AddDatabase("CatalogDB");


// Cache
var redis = builder.AddRedis("cache");

// DB Manager Apps

builder.AddProject<Catalog_Data_Manager>("catalog-db-mgr")
    .WithReference(catalogDb);


// API Apps

var catalogApi = builder.AddProject<Catalog_API>("catalog-api")
    .WithReference(catalogDb)
    .WithReference(redis)
    .WithReference(messaging);

// Apps

builder.AddProject<WebApp>("webapp")
    .WithReference(catalogApi)
    .WithReference(redis);

builder.AddProject<Projects.RabbitConsumer>("consumers").
        WithReference(messaging);

// Inject assigned URLs for Catalog API
catalogApi.WithEnvironment("CatalogOptions__PicBaseAddress", () => catalogApi.GetEndpoint("http").UriString);

builder.Build().Run();
