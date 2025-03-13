using System;
using BusinessLayer.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Interface;
using RepositoryLayer.Entity;

namespace BusinessLayer.Service
{
    public class AddressBookService : IAddressBookService
    {
        private readonly IAddressBookRepository _addressBookRepository;
            
        public AddressBookService(IAddressBookRepository addressBookRepository)
        {
            _addressBookRepository = addressBookRepository;
        }

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
            return _addressBookRepository.AddContact(contact);
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
