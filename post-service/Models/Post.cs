namespace post_service.Models;

public class Post
{
    public string Id { get; set; } = "";
    public string Content { get; set; } = "";

    public DateTime Posted { get; set; }
}