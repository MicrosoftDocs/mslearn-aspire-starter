using eShop.WebApp.Components.Catalog;

namespace eShop.WebApp.Services;

public partial class CatalogService(HttpClient httpClient, ILogger<CatalogService> logger)
{
    private readonly string remoteServiceBaseUrl = "api/v1/catalog/";

    public async Task<CatalogResult> GetCatalogItems(int pageIndex, int pageSize, int? brand, int? type)
    {
        var uri = GetAllCatalogItemsUri(remoteServiceBaseUrl, pageIndex, pageSize, brand, type);

        var result = await httpClient.GetFromJsonAsync<CatalogResult>(uri);
        return result ?? new(0, 0, 0, []);
    }

    public Task<CatalogItem?> GetCatalogItem(int id)
    {
        var uri = $"{remoteServiceBaseUrl}items/{id}";
        return httpClient.GetFromJsonAsync<CatalogItem>(uri);
    }

    public async Task<IEnumerable<CatalogBrand>> GetBrands()
    {
        var uri = $"{remoteServiceBaseUrl}catalogBrands";
        LogGetBrands(uri, logger);
        var result = await httpClient.GetFromJsonAsync<CatalogBrand[]>(uri);
        return result ?? [];
    }

    public async Task<IEnumerable<CatalogItemType>> GetTypes()
    {
        var uri = $"{remoteServiceBaseUrl}catalogTypes";
        var result = await httpClient.GetFromJsonAsync<CatalogItemType[]>(uri);
        return result ?? [];
    }

    private static string GetAllCatalogItemsUri(string baseUri, int pageIndex, int pageSize, int? brand, int? type)
    {
        // Build URLs like:
        //   [base]/items
        //   [base]/items/type/all
        //   [base]/items/type/123/brand/456
        //   [base]/items/type/123/brand/456?pageSize=9&pageIndex=2

        string filterPath;

        if (type.HasValue)
        {
            var brandPath = brand.HasValue ? brand.Value.ToString() : string.Empty;
            filterPath = $"/type/{type.Value}/brand/{brandPath}";

        }
        else if (brand.HasValue)
        {
            var brandPath = brand.HasValue ? brand.Value.ToString() : string.Empty;
            filterPath = $"/type/all/brand/{brandPath}";
        }
        else
        {
            filterPath = string.Empty;
        }

        return $"{baseUri}items{filterPath}?pageIndex={pageIndex}&pageSize={pageSize}";
    }

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Getting brands from URI: {uri}")]
    public static partial void LogGetBrands(string uri, ILogger logger);
}
