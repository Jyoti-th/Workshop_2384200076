using RepositoryLayer.Interface;
using RepositoryLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Service
{
    public class AddressBookRepository : IAddressBookRepository
    {
        private readonly AddressAppContext _context;

        public AddressBookRepository(AddressAppContext context)
        {
            _context = context;
        }

        // Get All Contacts
        public List<AddressBookEntity> GetAllContacts()
        {
            return _context.AddressBooks.ToList();
        }

        // Get Contact by ID
        public AddressBookEntity GetContactById(int id)
        {
            return _context.AddressBooks.FirstOrDefault(c => c.Id == id);
        }

        // Add Contact
        public AddressBookEntity AddContact(AddressBookEntity contact)
        {
            _context.AddressBooks.Add(contact);
            _context.SaveChanges();
            return contact;
        }

        //  Update Contact
        public AddressBookEntity UpdateContact(int id, AddressBookEntity contact)
        {
            _context.AddressBooks.Update(contact);
            _context.SaveChanges();
            return contact;
        }

        // Delete Contact
        public bool DeleteContact(int id)
        {
            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id);
            if (contact != null)
            {
                _context.AddressBooks.Remove(contact);
                _context.SaveChanges();
                return true;
            }
            return false;
        }


    }
}
