namespace Review.API.Model;

public class Review
{
  public string Id { get; set; } = "";
  public string ProductId { get; set; } = "";
  public string UserId { get; set; } = "";
  public string Name { get; set; } = "";
  public string Content { get; set; } = "";

  private int _rating = 1;
  public int Rating
  {
    get => _rating;
    set => _rating = value < 1 ? 1 : value > 5 ? 5 : value;
  }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

