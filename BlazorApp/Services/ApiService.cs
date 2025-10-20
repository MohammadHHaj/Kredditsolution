using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Shared.Model;

namespace BlazorApp.Services;

public class ApiService
{
    private readonly HttpClient http;
    private readonly IConfiguration configuration;
    private readonly string baseAPI;

    public ApiService(HttpClient http, IConfiguration configuration)
    {
        this.http = http;
        this.configuration = configuration;
        this.baseAPI = configuration["base_api"] ?? "http://localhost:5167/api/";
    }

    // Hent alle posts
    public async Task<Post[]> GetPosts()
    {
        string url = $"{baseAPI}posts";
        return await http.GetFromJsonAsync<Post[]>(url) ?? [];
    }

    // Hent et enkelt post med kommentarer
    public async Task<Post> GetPostAsync(int id)
    {
        string url = $"{baseAPI}posts/{id}";
        return await http.GetFromJsonAsync<Post>(url);
    }

    // Tilføj en kommentar og returnér det opdaterede Post-objekt
    public async Task<Post> AddComment(int postId, string text, string author)
    {
        string url = $"{baseAPI}posts/{postId}/comment";
        var data = new { Text = text, Author = author };

        HttpResponseMessage msg = await http.PostAsJsonAsync(url, data);
        string json = await msg.Content.ReadAsStringAsync();

        Post? updatedPost = JsonSerializer.Deserialize<Post>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return updatedPost!;
    }
    
    // Tilføj en post
    public async Task<Post[]> CreatePost(string title, string author)
    {
        string url = $"{baseAPI}posts";
        var postData = new { Title = title, Author = author };

        var response = await http.PostAsJsonAsync(url, postData);
        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Post[]>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    // Upvote post og returnér det opdaterede Post-objekt
    public async Task<Post> UpvotePost(int id)
    {
        string url = $"{baseAPI}posts/{id}/upvote";
        HttpResponseMessage msg = await http.PostAsJsonAsync(url, new { });
        string json = await msg.Content.ReadAsStringAsync();

        Post? updatedPost = JsonSerializer.Deserialize<Post>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return updatedPost!;
    }

    // Downvote post og returnér det opdaterede Post-objekt
    public async Task<Post> DownvotePost(int id)
    {
        string url = $"{baseAPI}posts/{id}/downvote";
        HttpResponseMessage msg = await http.PostAsJsonAsync(url, new { });
        string json = await msg.Content.ReadAsStringAsync();

        Post? updatedPost = JsonSerializer.Deserialize<Post>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return updatedPost!;
    }

    // Upvote kommentar og returnér opdateret Post
    public async Task<Post> UpvoteComment(int postId, int commentId)
    {
        string url = $"{baseAPI}posts/{postId}/comments/{commentId}/upvote";
        HttpResponseMessage msg = await http.PostAsJsonAsync(url, new { });
        string json = await msg.Content.ReadAsStringAsync();

        Post? updatedPost = JsonSerializer.Deserialize<Post>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return updatedPost!;
    }

    // Downvote kommentar og returnér opdateret Post
    public async Task<Post> DownvoteCommentAsync(int postId, int commentId)
    {
        string url = $"{baseAPI}posts/{postId}/comments/{commentId}/downvote";
        HttpResponseMessage msg = await http.PostAsJsonAsync(url, new { });
        string json = await msg.Content.ReadAsStringAsync();

        Post? updatedPost = JsonSerializer.Deserialize<Post>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return updatedPost!;
    }
}
