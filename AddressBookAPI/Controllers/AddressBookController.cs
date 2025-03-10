using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using System;

namespace AddressBook.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressBookController : ControllerBase
    {
        private readonly AddressAppContext _context;

        // Constructor Dependency Injection 
        public AddressBookController(AddressAppContext context)
        {
            _context = context;
        }

    

        // ✅ GET: Fetch all contacts
        [HttpGet]
        public ActionResult<List<AddressBookEntity>> GetAllContacts()
        {
            return _context.AddressBooks.ToList(); // 🟢 Simple synchronous approach
        }

        //  GET: Get contact by ID
        [HttpGet("{id}")]
        public ActionResult<AddressBookEntity> GetContactById(int id)
        {
            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id);
            if (contact == null)
                return NotFound();

            return contact;
        }

        // POST: Add a new contact
        [HttpPost]
        public ActionResult<AddressBookEntity> AddContact(AddressBookEntity contact)
        {
            _context.AddressBooks.Add(contact);
            _context.SaveChanges(); // 🟢 Direct DB Save
            return CreatedAtAction(nameof(GetContactById), new { id = contact.Id }, contact);
        }

        //  PUT: Update an existing contact
        [HttpPut("{id}")]
        public IActionResult UpdateContact(int id, AddressBookEntity updatedContact)
        {
            var existingContact = _context.AddressBooks.FirstOrDefault(c => c.Id == id);
            if (existingContact == null)
                return NotFound();

            existingContact.Name = updatedContact.Name;
            existingContact.PhoneNumber = updatedContact.PhoneNumber;
            existingContact.Email = updatedContact.Email;

            _context.SaveChanges(); // 🟢 Directly saving changes
            return NoContent();
        }

        //  DELETE: Remove a contact
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id);
            if (contact == null)
                return NotFound();

            _context.AddressBooks.Remove(contact);
            _context.SaveChanges(); // 🟢 Removing entry from DB
            return NoContent();
        }
    }
}