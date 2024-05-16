using Microsoft.Extensions.Options;
namespace Review.API;

public readonly struct ReviewServices(ReviewDbContext dbContext,  ILogger<ReviewServices> logger)
{
  public ReviewDbContext DbContext { get; } = dbContext;

  public ILogger<ReviewServices> Logger { get; } = logger;
};
