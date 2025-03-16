using NUnit.Framework;
using Moq;
using AutoMapper;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using RepositoryLayer.Interface;
using RepositoryLayer.Entity;
using ModelLayer.DTO;
using System;

namespace AddressBookTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IMapper> _mapperMock;
        private IUserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();

            _userService = new UserService(_userRepoMock.Object, _mapperMock.Object);
        }

        [Test]
        public void GetUserByEmail_ShouldReturnUserEntity_WhenUserExists()
        {
            // Arrange
            var email = "test@example.com";
            var userEntity = new UserEntity { Email = email, PasswordHash = "hashedpwd" };

            _userRepoMock.Setup(repo => repo.GetUserByEmail(email)).Returns(userEntity);

            // Act
            var result = _userService.GetUserByEmail(email);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(email, result.Email);
        }

        [Test]
        public void GetUserByEmail_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var email = "notfound@example.com";
            _userRepoMock.Setup(repo => repo.GetUserByEmail(email)).Returns((UserEntity)null);

            // Act
            var result = _userService.GetUserByEmail(email);

            // Assert
            Assert.IsNull(result);
        }
    }
}
