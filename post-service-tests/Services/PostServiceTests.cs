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

        var result = postService.AddPost(user.Id, post.Content);

        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Single(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal(post.Content, result.Content);
        Assert.Equal(user.Id, result.User.Id);
    }

    [Fact]
    public void AddPost_ContentEmpty()
    {
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

        var result = Assert.Throws<BadRequestException>(() =>
            postService.AddPost(user.Id, post.Content)
        );

        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Never);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("post content cannot be empty.", result.Message);
    }

    [Fact]
    public void AddPost_ContentTooLong()
    {
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        var post = new Post
        {
            Id = "test-id",
            Content = new string('a', 281),
        };

        var result = Assert.Throws<BadRequestException>(() =>
            postService.AddPost(user.Id, post.Content)
        );

        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Never);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("post content may not exceed 280 characters.", result.Message);
    }

    [Fact]
    public void AddPost_UserNotFound()
    {
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

        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(null as User);

        var result = Assert.Throws<BadRequestException>(() =>
            postService.AddPost(user.Id, post.Content)
        );

        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
        Assert.Equal("User with id 'test-id' doesn't exist.", result.Message);
    }

    [Fact]
    public void RemovePost_Success()
    {
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var post = new Post
        {
            Id = "test-id",
            Content = new string('a', 280),
        };

        unitOfWork.Setup(x => x.Posts.GetById(post.Id)).Returns(post);

        postService.DeletePost(post.Id);

        unitOfWork.Verify(x => x.Posts.GetById(post.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Single(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
    }

    [Fact]
    public void RemovePost_NotFound()
    {
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var post = new Post
        {
            Id = "test-id",
            Content = new string('a', 280),
        };

        unitOfWork.Setup(x => x.Posts.GetById(post.Id)).Returns(null as Post);

        Assert.Throws<NotFoundException>(() =>
            postService.DeletePost(post.Id)
        );

        unitOfWork.Verify(x => x.Posts.GetById(post.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Empty(eventService.Invocations.Where(i => i.Method.Name == "Publish"));
    }

    [Fact]
    public void GetPostById_Success()
    {
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var post = new Post
        {
            Id = "test-id",
            Content = new string('a', 280),
        };

        unitOfWork.Setup(x => x.Posts.GetById(post.Id)).Returns(post);

        var result = postService.GetPostById(post.Id);

        unitOfWork.Verify(x => x.Posts.GetById(post.Id), Times.Once);

        Assert.Equal(post, result);
    }

    [Fact]
    public void GetPostById_NotFound()
    {
        var postService = new PostService(unitOfWork.Object, eventService.Object);

        var post = new Post
        {
            Id = "test-id",
            Content = new string('a', 280),
        };

        unitOfWork.Setup(x => x.Posts.GetById(post.Id)).Returns(null as Post);

        var result = Assert.Throws<NotFoundException>(() =>
            postService.GetPostById(post.Id)
        );

        unitOfWork.Verify(x => x.Posts.GetById(post.Id), Times.Once);

        Assert.Equal($"Post with id '{post.Id}' doesn't exist.", result.Message);
    }
}
