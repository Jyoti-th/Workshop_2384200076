using NUnit.Framework;
using Moq;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using RepositoryLayer.Interface;
using RepositoryLayer.Entity;
using System.Collections.Generic;
using RepositoryLayer.Context;

namespace AddressBookTests
{
    [TestFixture] // NUnit Test Class
    public class AddressBookServiceTests
    {
        private Mock<IAddressBookRepository> _addressRepoMock;
        private Mock<AddressAppContext> _dbContextMock;
        private Mock<RabbitMQService> _rabbitMQMock;
        private IAddressBookService _addressBookService;

        [SetUp] // Har test ke pehle chalega
        public void Setup()
        {
            _addressRepoMock = new Mock<IAddressBookRepository>();
            _dbContextMock = new Mock<AddressAppContext>();
            _rabbitMQMock = new Mock<RabbitMQService>();

            _addressBookService = new AddressBookService(_addressRepoMock.Object, _dbContextMock.Object, _rabbitMQMock.Object);
        }

        [Test]
        public void GetAllContacts_ShouldReturnListOfContacts()
        {
            // Arrange
            var contacts = new List<AddressBookEntity>
            {
                new AddressBookEntity { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new AddressBookEntity { Id = 2, Name = "Jane Doe", Email = "jane@example.com" }
            };

            _addressRepoMock.Setup(repo => repo.GetAllContacts()).Returns(contacts);

            // Act
            var result = _addressBookService.GetAllContacts();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}
