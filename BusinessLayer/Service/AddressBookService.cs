using System;
using BusinessLayer.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Interface;
using RepositoryLayer.Entity;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;

namespace BusinessLayer.Service
{
    public class AddressBookService : IAddressBookService
    {
        private readonly IAddressBookRepository _addressBookRepository;
 
        private readonly AddressAppContext _context;

        private readonly RabbitMQService _rabbitMQService;

        public AddressBookService(IAddressBookRepository addressBookRepository,  AddressAppContext context, RabbitMQService rabbitMQService)
        {
            _addressBookRepository = addressBookRepository;
           
            _context = context;

            _rabbitMQService = rabbitMQService;
        }

     
        //CRUD Operations
        public List<AddressBookEntity> GetAllContacts()
        {
            return _addressBookRepository.GetAllContacts();
        }

        public AddressBookEntity GetContactById(int id)
        {
            return _addressBookRepository.GetContactById(id);
        }

        public AddressBookEntity AddContact(AddressBookEntity contact)
        {
            var addedContact = _addressBookRepository.AddContact(contact);

            // RabbitMQ me event publish karo
            var message = $"New contact added: {addedContact.Name}, {addedContact.Email}";
            _rabbitMQService.PublishMessage("ContactAddedQueue", message);

            return addedContact;
        }

        public AddressBookEntity UpdateContact(int id, AddressBookEntity contact)
        {
            return _addressBookRepository.UpdateContact(id, contact);
        }

        public bool DeleteContact(int id)
        {
            return _addressBookRepository.DeleteContact(id);
        }
    }
}
