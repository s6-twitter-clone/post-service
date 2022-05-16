namespace post_service.Models.Events;

public class UpdatePostEvent
{
    public string Id { get; set; } = "";
    public string Content { get; set; } = "";
}
