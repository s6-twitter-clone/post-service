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
            Content = post.Content
        };
    }

    [HttpDelete("{id}")]
    [Authorize]
    public void RemovePost(string id)
    {
        postService.DeletePost(id);
    }

}
