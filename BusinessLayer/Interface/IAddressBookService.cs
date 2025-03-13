using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IAddressBookService
    {
        public List<AddressBookEntity> GetAllContacts();
        public AddressBookEntity GetContactById(int id);
        public AddressBookEntity AddContact(AddressBookEntity contact);
        public AddressBookEntity UpdateContact(int id, AddressBookEntity contact);
        public bool DeleteContact(int id);
    }
}
