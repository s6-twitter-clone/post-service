namespace post_service.Dtos;

public class PostDTO
{
    public string Id { get; set; } = "";
    public string Content { get; set; } = "";

    public UserDTO User { get; set; } = new UserDTO();
}
