using WebApi.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Model;
using WebApi.Data;


namespace WebApi;

public class Dataservice
{
    private PostContext db { get; }

    public Dataservice(PostContext db)
    {
        this.db = db;
    }

    /// <summary>
    /// Seeder nyt data i databasen hvis nødvendigt.
    /// </summary>
    public void SeedData()
    {
        if (!db.Posts.Any())
        {
            var post1 = new Post
            {
                Title = "Velkommen til Kreddit!",
                Author = "Admin",
                Date = DateTime.UtcNow,
                Upvotes = 3,
                Downvotes = 0,
                Comments = new List<Comment>
                {
                    new Comment { Text = "Spændende projekt!", Author = "Mette", Date = DateTime.UtcNow, Upvotes = 1, Downvotes = 0 },
                    new Comment { Text = "Glæder mig til at følge med", Author = "Søren", Date = DateTime.UtcNow, Upvotes = 2, Downvotes = 0 }
                }
            };

            var post2 = new Post
            {
                Title = "Hvad synes I om Blazor?",
                Author = "Mohammad",
                Date = DateTime.UtcNow,
                Upvotes = 5,
                Downvotes = 1,
                Comments = new List<Comment>
                {
                    new Comment { Text = "Det er nice til små projekter!", Author = "Kristian", Date = DateTime.UtcNow, Upvotes = 2, Downvotes = 0 }
                }
            };

            db.Posts.AddRange(post1, post2);
            db.SaveChanges();
        }
    }

    // --------- Posts ---------
    public List<Post> GetPosts()
    {
        return db.Posts.Include(p => p.Comments).ToList();
    }

    public Post? GetPost(int id)
    {
        return db.Posts.Include(p => p.Comments)
                       .FirstOrDefault(p => p.PostId == id);
    }

    public string CreatePost(string title, string? text, string? url, string author)
    {
        var post = new Post
        {
            Title = title,
            Author = author,
            Date = DateTime.UtcNow
        };

        db.Posts.Add(post);
        db.SaveChanges();
        return "Post Oprettet";
    }

    public string UpvotePost(int id)
    {
        var post = db.Posts.Find(id);
        if (post == null) return "Post Blev ikke fundet";

        post.Upvotes++;
        db.SaveChanges();
        return "Post upvoted";
    }

    public string DownvotePost(int id)
    {
        var post = db.Posts.Find(id);
        if (post == null) return "Post blev ikke fundet";

        post.Downvotes++;
        db.SaveChanges();
        return "Post downvoted";
    }

    // --------- Comments ---------
    public string AddComment(int postId, string text, string author)
    {
        var post = db.Posts
            .Include(p => p.Comments)
            .FirstOrDefault(p => p.PostId == postId);

        if (post == null)
            return "Post blev ikke fundet";

        var comment = new Comment
        {
            Text = text,
            Author = author,
            Date = DateTime.UtcNow,
            Upvotes = 0,
            Downvotes = 0
        };

        post.Comments ??= new List<Comment>();
        post.Comments.Add(comment);

        db.SaveChanges();
        return "Kommentar oprettet";
    }


    public string UpvoteComment(int postId, int commentId)
    {
        var comment = db.Comments.FirstOrDefault(c => c.CommentId == commentId && c.PostId == postId);
        if (comment == null) return "Kommentaren blev ikke fundet";

        comment.Upvotes++;
        db.SaveChanges();
        return "Kommentaren upvoted";
    }

    public string DownvoteComment(int postId, int commentId)
    {
        var comment = db.Comments.FirstOrDefault(c => c.CommentId == commentId && c.PostId == postId);
        if (comment == null) return "Kommentaren blev ikke fundet";

        comment.Downvotes++;
        db.SaveChanges();
        return "Kommentaren downvoted";
    }
}

