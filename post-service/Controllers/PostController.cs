using Microsoft.AspNetCore.Mvc;
using post_service.Dtos;
using post_service.Services;
using post_service.Interfaces;

namespace post_service.Controllers;

[ApiController]
[Route("[controller]s")]
public class PostController : ControllerBase
{
    private readonly PostService postService;
    public PostController(PostService postService)
    {
        this.postService = postService;
    }


    [HttpPost]
    public PostDTO AddPost(PostDTO post)
    {
        var newPost = postService.AddPost(post.Content);
        return new PostDTO
        {
            Id = newPost.Id,
            Content = newPost.Content
        };
    }

    [HttpGet]
    public PostDTO GetPost(string postId)
    {
        var post = postService.GetPostById(postId);

        return new PostDTO
        {
            Id = post.Id,
            Content = post.Content
        };
    }

    [HttpDelete]
    public void RemovePost(string postId)
    {
        postService.DeletePost(postId);
    }

}
