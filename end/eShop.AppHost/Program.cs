using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Databases

var postgres = builder.AddPostgres("postgres").WithPgAdmin();
var catalogDb = postgres.AddDatabase("CatalogDB");
var mongo = builder.AddMongoDB("mongo")
  .WithMongoExpress()
  .AddDatabase("BasketDB");

// Identity Providers

var idp = builder.AddKeycloakContainer("idp", tag: "23.0")
    .ImportRealms("../Keycloak/data/import");

// DB Manager Apps

builder.AddProject<Catalog_Data_Manager>("catalog-db-mgr")
    .WithReference(catalogDb);

// API Apps

var catalogApi = builder.AddProject<Catalog_API>("catalog-api")
    .WithReference(catalogDb);

var basketApi = builder.AddProject<Basket_API>("basket-api")
        .WithReference(mongo)
        .WithReference(idp);

// Apps

var webApp = builder.AddProject<WebApp>("webapp")
    .WithReference(catalogApi)
    .WithReference(basketApi)
    .WithReference(idp)
    // Force HTTPS profile for web app (required for OIDC operations)
    .WithLaunchProfile("https");

// Inject the project URLs for Keycloak realm configuration
idp.WithEnvironment("WEBAPP_HTTP", () => webApp.GetEndpoint("http").Value);
idp.WithEnvironment("WEBAPP_HTTPS", () => webApp.GetEndpoint("https").Value);

// Inject assigned URLs for Catalog API
catalogApi.WithEnvironment("CatalogOptions__PicBaseAddress", () => catalogApi.GetEndpoint("http").Value);

builder.Build().Run();
