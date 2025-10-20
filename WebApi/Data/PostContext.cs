using Microsoft.EntityFrameworkCore;
using Shared.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Model; // så du kan bruge Post og Comment

namespace WebApi.Data; // 👈 vigtigt!

public class PostContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();

    public PostContext(DbContextOptions<PostContext> options)
        : base(options) { }
}
