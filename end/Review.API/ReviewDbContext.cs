using Microsoft.EntityFrameworkCore;
using Review.API.Model;

public class ReviewDbContext(DbContextOptions<ReviewDbContext> options) : DbContext(options)
{
  public DbSet<Review> Reviews { get; set; }

}
