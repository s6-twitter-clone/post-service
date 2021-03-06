using Microsoft.AspNetCore.Mvc;
using post_service.Dtos;
using post_service.Services;
using post_service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace post_service.Controllers;

[ApiController]
[Route("")]
public class PostController : ControllerBase
{
    private readonly PostService postService;
    public PostController(PostService postService)
    {
        this.postService = postService;
    }


    [HttpGet]
    public IEnumerable<PostDTO> GetPosts([FromQuery] string userId, [FromQuery] int offset = 0, [FromQuery] int count = 5)
    {
        var posts = postService.GetPosts(userId, count, offset);

        return posts.Select(post => new PostDTO
        {
            Id = post.Id,
            Content = post.Content,
            User = new UserDTO
            {
                Id = post.User.Id,
                DisplayName = post.User.DisplayName
            }
        });
    }



    [HttpPost]
    [Authorize]
    public PostDTO AddPost(CreatePostDTO post)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var newPost = postService.AddPost(userId, post.Content);

        return new PostDTO
        {
            Id = newPost.Id,
            Content = newPost.Content
        };
    }

    [HttpGet("{id}")]
    public PostDTO GetPost(string id)
    {
        var post = postService.GetPostById(id);

        return new PostDTO
        {
            Id = post.Id,
            Content = post.Content,
            User = new UserDTO
            {
                Id = post.User.Id,
                DisplayName = post.User.DisplayName
            }
        };
    }

    [HttpDelete("{id}")]
    [Authorize]
    public void RemovePost(string id)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        postService.DeletePost(id, userId);
    }

}
