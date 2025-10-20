namespace Shared.Model;

public class Comment
{
    public int CommentId { get; set; }
    public string? Text { get; set; }
    public string? Author { get; set; }
    public DateTime Date { get; set; }
    public int? Upvotes { get; set; }
    public int? Downvotes { get; set; }
    
    public int PostId { get; set; }
    public Post? Post { get; set; }
}