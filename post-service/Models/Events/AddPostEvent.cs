namespace post_service.Models.Events;

public class AddPostEvent
{
    public string Id { get; set; } = "";
    public string Content { get; set; } = "";

    public DateTime Posted { get; set; }

    public string UserId { get; set; } = "";
}
