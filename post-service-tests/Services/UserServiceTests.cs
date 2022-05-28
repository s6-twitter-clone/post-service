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

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> unitOfWork = new();

    public UserServiceTests()
    {
        var user = new User();
        unitOfWork.Setup(x => x.Users.Add(user)).Returns(user);

        unitOfWork.Setup(x => x.Commit()).Returns(0);
    }

    [Fact]
    public void GetById_Success()
    {
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(user);

        var result = userService.GetById(user.Id);

        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);

        Assert.Equal(user, result);
    }

    [Fact]
    public void GetById_NotFound()
    {
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(null as User);

        var result = Assert.Throws<NotFoundException>(() =>
            userService.GetById(user.Id)
        );

        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);

        Assert.Equal($"User with id '{user.Id}' doesn't exist.", result.Message);
    }


    [Fact]
    public void Add_Success()
    {
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        unitOfWork.Setup(x => x.Commit()).Returns(1);

        var result = userService.Add(user.Id, user.DisplayName);    
    }

    [Fact]
    public void Add_CouldNotAdd()
    {
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        var result = Assert.Throws<InternalServerErrorException>(() =>
            userService.Add(user.Id, user.DisplayName)
        );

        Assert.Equal($"User with id '{user.Id}' could not be added.", result.Message);
    }

    [Fact]
    public void Update_Success()
    {
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        var newUser = new User
        {
            Id = "test-id",
            DisplayName = "new name"
        };

        unitOfWork.Setup(x => x.Commit()).Returns(1);
        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(user);

        var result = userService.Update(user.Id, newUser.DisplayName);

        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Equal(newUser.DisplayName, result.DisplayName);
    }

    [Fact]
    public void Update_NotFound()
    {
        var userService = new UserService(unitOfWork.Object);

        var newUser = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        unitOfWork.Setup(x => x.Commit()).Returns(1);
        unitOfWork.Setup(x => x.Users.GetById(newUser.Id)).Returns(null as User);

        var result = Assert.Throws<NotFoundException>(() => 
            userService.Update(newUser.Id, newUser.DisplayName)
        );

        unitOfWork.Verify(x => x.Users.GetById(newUser.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Never);

        Assert.Equal($"User with id '{newUser.Id}' doesn't exist.", result.Message);
    }

    [Fact]
    public void Update_CouldNotUpdate()
    {
        var userService = new UserService(unitOfWork.Object);

        var user = new User
        {
            Id = "test-id",
            DisplayName = "name",
        };

        var newUser = new User
        {
            Id = "test-id",
            DisplayName = "new name"
        };

        unitOfWork.Setup(x => x.Users.GetById(user.Id)).Returns(user);

        var result = Assert.Throws<InternalServerErrorException>(() =>
            userService.Update(newUser.Id, newUser.DisplayName)
        );

        unitOfWork.Verify(x => x.Users.GetById(user.Id), Times.Once);
        unitOfWork.Verify(x => x.Commit(), Times.Once);

        Assert.Equal($"User with id '{newUser.Id}' could not be updated.", result.Message);
    }
}
