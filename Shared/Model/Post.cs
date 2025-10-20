namespace Shared.Model;

public class Post
{
    public int PostId { get; set; }
    public string? Title { get; set; }
    public DateTime? Date { get; set; }
    public string? Author { get; set; }
    public int? Upvotes { get; set; } = 0;
    public int? Downvotes { get; set; } = 0;
    
    public List<Comment> Comments { get; set; } = new();
}