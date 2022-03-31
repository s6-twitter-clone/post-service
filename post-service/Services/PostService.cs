using post_service.Exceptions;
using post_service.Interfaces;
using post_service.Models;

namespace post_service.Services;

public class PostService
{
    private readonly IUnitOfWork unitOfWork;
    public PostService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public Post AddPost(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            throw new BadRequestException("post content cannot be empty.");
        }

        if (content.Length > 280)
        {
            throw new BadRequestException("post content may not exceed 280 characters.");
        }

        var post = new Post
        {
            Id = Guid.NewGuid().ToString(),
            Content = content,
            Posted = DateTime.UtcNow
        };


        unitOfWork.Posts.Add(post);
            
        unitOfWork.Commit();

        return post;
    }

    public Post GetPostById(string id)
    {
        var post = unitOfWork.Posts.GetById(id);

        if (post is null)
        {
            throw new NotFoundException($"Post with id '{id}' doesn't exist.");
        }

        return post;
    }

    public void DeletePost(string id)
    {
        var post = GetPostById(id);

        unitOfWork.Posts.Remove(post);
        unitOfWork.Commit();
    }
}
