using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Review.API;

namespace Microsoft.AspNetCore.Builder;

public static class ReviewApi
{
  private static readonly FileExtensionContentTypeProvider _fileContentTypeProvider = new();

  public static IEndpointRouteBuilder MapReviewApi(this IEndpointRouteBuilder app)
  {
    // Routes for querying reviews.
    app.MapGet("/items", GetAllItems);
    app.MapGet("/items/fot", GetItemsForProductId);
    app.MapGet("/items/{id:int}", GetItemByReviewId);

    return app;
  }

  public static async Task<Results<Ok<PaginatedItems<CatalogItem>>, BadRequest<string>>> GetAllItems(
      [AsParameters] PaginationRequest paginationRequest,
      [AsParameters] ReviewServices services)
  {
    var pageSize = paginationRequest.PageSize;
    var pageIndex = paginationRequest.PageIndex;

    var totalItems = await services.DbContext.Reviews.;

    var itemsOnPage = await services.DbContext.CatalogItems
        .OrderBy(c => c.Name)
        .Skip(pageSize * pageIndex)
        .Take(pageSize)
        .AsNoTracking()
        .ToListAsync();

    ChangeUriPlaceholder(services.Options.Value, itemsOnPage);

    return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
  }

  public static async Task<Ok<List<CatalogItem>>> GetItemsForProductId(
      [AsParameters] CatalogServices services,
      int[] ids)
  {
    var items = await services.DbContext.CatalogItems
        .Where(item => ids.Contains(item.Id))
        .AsNoTracking()
        .ToListAsync();

    ChangeUriPlaceholder(services.Options.Value, items);

    return TypedResults.Ok(items);
  }

  public static async Task<Results<Ok<CatalogItem>, NotFound, BadRequest<string>>> GetItemByReviewId(
      [AsParameters] CatalogServices services,
      int id)
  {
    if (id <= 0)
    {
      return TypedResults.BadRequest("Id is not valid.");
    }

    var item = await services.DbContext.CatalogItems
        .Include(ci => ci.CatalogBrand)
        .AsNoTracking()
        .SingleOrDefaultAsync(ci => ci.Id == id);

    if (item == null)
    {
      return TypedResults.NotFound();
    }

    item.PictureUri = services.Options.Value.GetPictureUrl(item.Id);

    return TypedResults.Ok(item);
  }

}
