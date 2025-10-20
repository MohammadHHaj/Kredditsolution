using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

// ---------- KONFIGURATION ----------

// CORS så Blazor må tilgå API'et
var AllowBlazor = "_AllowBlazor";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowBlazor, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JSON håndtering uden reference loops
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// DbContext (SQLite)
builder.Services.AddDbContext<PostContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Tilføj DataService
builder.Services.AddScoped<Dataservice>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetRequiredService<Dataservice>();
    dataService.SeedData();
}


app.UseHttpsRedirection();
app.UseCors(AllowBlazor);

// ---------- API ENDPOINTS ----------

// Hent alle posts
// Hent max 50 nyeste posts
app.MapGet("/api/posts", (Dataservice service) =>
{
    return Results.Ok(
        service.GetPosts()
            .OrderByDescending(p => p.Date) // sorter efter nyeste
            .Take(50)                       // maks 50 posts
    );
});


// Hent ét post
app.MapGet("/api/posts/{id}", (Dataservice service, int id) =>
{
    var post = service.GetPost(id);
    return post is not null ? Results.Ok(post) : Results.NotFound();
});

// Opret nyt post
app.MapPost("/api/posts", (Dataservice service, NewPost data) =>
{
    service.CreatePost(data.Title, null, null, data.Author);
    var posts = service.GetPosts();
    return Results.Ok(posts);
});

// Upvote post
app.MapPost("/api/posts/{id}/upvote", (Dataservice service, int id) =>
{
    service.UpvotePost(id);
    var updatedPost = service.GetPost(id);
    return Results.Ok(updatedPost);
});

// Downvote post
app.MapPost("/api/posts/{id}/downvote", (Dataservice service, int id) =>
{
    service.DownvotePost(id);
    var updatedPost = service.GetPost(id);
    return Results.Ok(updatedPost);
});

// Tilføj kommentar
app.MapPost("/api/posts/{postId}/comment", (Dataservice service, int postId, NewComment data) =>
{
    service.AddComment(postId, data.Text, data.Author);
    var updatedPost = service.GetPost(postId);
    return Results.Ok(updatedPost);
});

// Upvote kommentar
app.MapPost("/api/posts/{postId}/comments/{commentId}/upvote", (Dataservice service, int postId, int commentId) =>
{
    service.UpvoteComment(postId, commentId);
    var updatedPost = service.GetPost(postId);
    return Results.Ok(updatedPost);
});

// Downvote kommentar
app.MapPost("/api/posts/{postId}/comments/{commentId}/downvote", (Dataservice service, int postId, int commentId) =>
{
    service.DownvoteComment(postId, commentId);
    var updatedPost = service.GetPost(postId);
    return Results.Ok(updatedPost);
});

app.Run();

// ---------- RECORD TYPES ----------
record NewPost(string Title, string Author);
record NewComment(string Text, string Author);
