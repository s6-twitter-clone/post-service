using post_service.Exceptions;
using post_service.Interfaces;
using post_service.Models;
using post_service.Models.Events;

namespace post_service.Services;

public class PostService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IEventService eventService;

    public PostService(IUnitOfWork unitOfWork, IEventService eventService)
    {
        this.unitOfWork = unitOfWork;
        this.eventService = eventService;
    }

    public IEnumerable<Post> GetPosts(string userId, int count, int offset)
    {
        if(offset < 0)
        {
            throw new BadRequestException("Post offset must be positive");
        }

        if(count <= 0 || count > 10)
        {
            throw new BadRequestException("Post count must be between 1 and 10");
        }

        var user = unitOfWork.Users.GetById(userId);

        if (user is null)
        {
            throw new BadRequestException($"User with id '{userId}' doesn't exist.");
        }



        return unitOfWork.Posts.GetAll().Where(x => x.UserId == userId).Skip(offset).Take(count);
    }

    public Post AddPost(string userId, string content)
    {

        if (string.IsNullOrEmpty(content))
        {
            throw new BadRequestException("post content cannot be empty.");
        }

        if (content.Length > 280)
        {
            throw new BadRequestException("post content may not exceed 280 characters.");
        }

        var user = unitOfWork.Users.GetById(userId);

        if(user is null)
        {
            throw new BadRequestException($"User with id '{userId}' doesn't exist.");
        }

        var post = new Post
        {
            Id = Guid.NewGuid().ToString(),
            Content = content,
            Posted = DateTime.UtcNow,
            User = user,
        };


        unitOfWork.Posts.Add(post);

        eventService.Publish(exchange: "post-exchange", topic: "post-added", new AddPostEvent
        {
            Id = post.Id,
            Content = post.Content,
            UserId = userId,
            Posted = post.Posted
        });

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

    public void DeletePost(string id, string userId)
    {
        var post = GetPostById(id);

        if(post.UserId != userId)
        {
            throw new ForbiddenException($"User with id '{userId}' is not authorized to delete post with id {id}.");
        }

        unitOfWork.Posts.Remove(post);

        eventService.Publish(exchange: "post-exchange", topic: "post-deleted", id);

        unitOfWork.Commit();
    }
}
