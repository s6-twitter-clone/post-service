using Moq;
using post_service.Exceptions;
using post_service.Interfaces;
using post_service.Models;
using post_service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace post_service_tests.Services;

public class PostServiceTests
{
    private readonly Mock<IUnitOfWork> unitOfWork = new();
    private readonly Mock<IEventService> eventService = new();

    public PostServiceTests()
    {
        var post = new Post();
        unitOfWork.Setup(x => x.Posts.Add(post)).Returns(post);

        unitOfWork.Setup(x => x.Commit()).Returns(0);
    }

    [Fact]
    public void AddPost_Success()
    {
        // Arrange
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        var post = new Post
        {
            Id = "test-id",
            Content = new string('a', 280),

        };

        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(user);

        // Act
        var result = postService.AddPost(user.Id, post.Content);

        // Assert
        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Single(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal(post.Content, result.Content);
        Assert.Equal(user.Id, result.User.Id);
    }

    [Fact]
    public void AddPost_ContentEmpty()
    {
        // Arrange
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        var post = new Post
        {
            Id = "test-id",
            Content = "",
        };

        // Act
        var result = Assert.Throws<BadRequestException>(() =>
            postService.AddPost(user.Id, post.Content)
        );

        // Assert
        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Never);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("post content cannot be empty.", result.Message);
    }

    [Fact]
    public void AddPost_ContentTooLong()
    {
        // Arrange
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var userId = "test-id";
        var postContent = new string('a', 281);

        // Act
        var result = Assert.Throws<BadRequestException>(() =>
            postService.AddPost(userId, postContent)
        );

        // Assert
        unitOfWork.Verify(x => x.Users.GetById(userId), Times.Never);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("post content may not exceed 280 characters.", result.Message);
    }

    [Fact]
    public void AddPost_UserNotFound()
    {
        // Arrange
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var userId = "test-id";
        var postContent = new string('a', 280);

        unitOfWork.Setup(x => x.Users.GetById(userId)).Returns(null as User);
        
        // Act
        var result = Assert.Throws<BadRequestException>(() =>
            postService.AddPost(userId, postContent)
        );

        // Assert
        unitOfWork.Verify(x => x.Users.GetById(userId), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("User with id 'test-id' doesn't exist.", result.Message);
    }

    [Fact]
    public void RemovePost_Success()
    {
        // Arrange
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var userId = "test-id";

        var post = new Post
        {
            Id = "test-id",
            Content = new string('a', 280),
            UserId = userId,
        };

        unitOfWork.Setup(x => x.Posts.GetById(post.Id)).Returns(post);

        // Act
        postService.DeletePost(post.Id, userId);

        // Assert
        unitOfWork.Verify(x => x.Posts.GetById(post.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Single(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
    }

    [Fact]
    public void RemovePost_NotFound()
    {
        // Arrange
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var userId = "test-id";
        var postId = "test-id";

        unitOfWork.Setup(x => x.Posts.GetById(postId)).Returns(null as Post);

        // Act
        Assert.Throws<NotFoundException>(() =>
            postService.DeletePost(postId, userId)
        );

        // Assert
        unitOfWork.Verify(x => x.Posts.GetById(postId), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
    }

    [Fact]
    public void RemovePost_NotPoster()
    {
        // Arrange
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var posterId = "poster";
        var deleterId = "deleter";

        var post = new Post
        {
            Id = "test-id",
            UserId = posterId
        };

        unitOfWork.Setup(x => x.Posts.GetById(post.Id)).Returns(post);

        // Act
        var result = Assert.Throws<ForbiddenException>(() =>
            postService.DeletePost(post.Id, deleterId)
        );

        // Assert
        unitOfWork.Verify(x => x.Posts.GetById(post.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal($"User with id '{deleterId}' is not authorized to delete post with id {post.Id}.", result.Message);
    }

    [Fact]
    public void GetPostById_Success()
    {
        // Arrange
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var post = new Post
        {
            Id = "test-id",
            Content = new string('a', 280),
        };

        unitOfWork.Setup(x => x.Posts.GetById(post.Id)).Returns(post);

        // Act
        var result = postService.GetPostById(post.Id);

        // Assert
        unitOfWork.Verify(x => x.Posts.GetById(post.Id), Times.Once);

        Assert.Equal(post, result);
    }

    [Fact]
    public void GetPostById_NotFound()
    {
        // Arrange
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var post = new Post
        {
            Id = "test-id",
            Content = new string('a', 280),
        };

        unitOfWork.Setup(x => x.Posts.GetById(post.Id)).Returns(null as Post);

        // Act
        var result = Assert.Throws<NotFoundException>(() =>
            postService.GetPostById(post.Id)
        );

        // Assert
        unitOfWork.Verify(x => x.Posts.GetById(post.Id), Times.Once);

        Assert.Equal($"Post with id '{post.Id}' doesn't exist.", result.Message);
    }
}
