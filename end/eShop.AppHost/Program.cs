using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Databases

var catalogDB = builder.AddPostgres("postgres")
  .WithBindMount("../Catalog.API/Seed", "/docker-entrypoint-initdb.d")
  .WithEnvironment("POSTGRES_DB", "postgres")
  .WithPgAdmin();
var mongo = builder.AddMongoDB("mongo")
  .WithMongoExpress()
  .AddDatabase("BasketDB");

var cosmosdbService = builder.AddAzureCosmosDB("cdb")
                             .AddDatabase("cosmosdb")
                             .RunAsEmulator();

// Identity Providers

var idp = builder.AddKeycloakContainer("idp", tag: "23.0")
    .ImportRealms("../Keycloak/data/import");

// API Apps

var catalogApi = builder.AddProject<Catalog_API>("catalog-api")
    .WithReference(catalogDB);

var basketApi = builder.AddProject<Basket_API>("basket-api")
        .WithReference(mongo)
        .WithReference(idp);

var reviewApi = builder.AddProject<Review_API>("review-api")
    .WithReference(cosmosdbService);

// Apps

var webApp = builder.AddProject<WebApp>("webapp")
    .WithReference(catalogApi)
    .WithReference(basketApi)
    .WithReference(reviewApi)
    .WithReference(idp)
    // Force HTTPS profile for web app (required for OIDC operations)
    .WithLaunchProfile("https");

// Inject the project URLs for Keycloak realm configuration
idp.WithEnvironment("WEBAPP_HTTP", () => webApp.GetEndpoint("http").Value);
idp.WithEnvironment("WEBAPP_HTTPS", () => webApp.GetEndpoint("https").Value);

// Inject assigned URLs for Catalog API
catalogApi.WithEnvironment("CatalogOptions__PicBaseAddress", () => catalogApi.GetEndpoint("http").Value);

builder.Build().Run();
