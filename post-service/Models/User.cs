namespace post_service.Models;

public class User
{
    public string Id { get; set; } = "";
    public string DisplayName { get; set; } = "";

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
