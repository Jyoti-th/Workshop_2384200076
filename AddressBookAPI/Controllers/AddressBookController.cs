using AutoMapper;
using BusinessLayer.Service;
using BusinessLayer.Interface;
using BusinessLayer.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace AddressBook.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBookService _addressBookService;
        private readonly IMapper _mapper; // AutoMapper Inject kiya
        private readonly RedisCacheService _redisCacheService;
    

        // Constructor Dependency Injection 
        public AddressBookController(IAddressBookService addressBookService, IMapper mapper, RedisCacheService redisCacheService)
        {
            _addressBookService = addressBookService;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
       

        }


        /// <summary>
        /// Gets all contacts from the Address Book.
        /// </summary>
        /// <returns> Returns a list of contacts </returns>
        [HttpGet]
        public ActionResult<List<AddressBookEntity>> GetAllContacts()
        {
            return _addressBookService.GetAllContacts(); // 🟢 Simple synchronous approach
        }

        /// <summary>
        /// Get a Contact from the AddressBook using ID
        /// </summary>
        /// <param name="id">The ID of the contactto get detail</param>
        /// <returns>Returns the searching contact</returns>
        [HttpGet("{id}")]
        public ActionResult<AddressBookEntity> GetContactById(int id)
        {
            var contact = _addressBookService.GetContactById(id);
            return contact;
           
        }

        /// <summary>
        /// Adds a new contact to the Address Book.
        /// </summary>
        /// <param name="contactDTO">The contact details to add</param>
        /// <returns>Returns the added contact</returns>
        [HttpPost]
        public ActionResult<AddressBookEntity> AddContact(AddressBookDTO contactDTO)
        {
            var validator = new AddressBookValidator();
            var validationResult = validator.Validate(contactDTO);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            //AutoMapper ka use karke DTO ko Entity me convert kar rahe h
            var contact = _mapper.Map<AddressBookEntity>(contactDTO);
            var result = _addressBookService.AddContact(contact);

            
            return CreatedAtAction(nameof(GetContactById), new { id = result.Id }, result);
        }


        /// <summary>
        /// Updates an existing contact.
        /// </summary>
        /// <param name="id">The ID of the contact to update</param>
        /// <param name="contactDTO">Updated contact details</param>
        /// <returns>Returns the updated contact</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateContact(int id, [FromBody] AddressBookDTO contactDTO)
        {
            // Validate DTO using FluentValidation
            var validator = new AddressBookValidator();
            var validationResult = validator.Validate(contactDTO);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors); 
            }

            //  Get the existing contact
            var existingContact = _addressBookService.GetContactById(id);
            if (existingContact == null)
            {
                return NotFound("Contact not found!"); 
            }

            // mapping  DTO to  Entity 
            _mapper.Map(contactDTO, existingContact);

            // 
            var result = _addressBookService.UpdateContact(id, existingContact);

            return Ok(result); 
        }


        /// <summary>
        /// Deletes a contact from the Address Book
        /// </summary>
        /// <param name="id">The ID of the contact to delete</param>
        /// <returns>Returns success or failure</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            var contact = _addressBookService.DeleteContact(id);
            return Ok(contact);
        }

        /// <summary>
        /// Retrieves all contacts from Redis cache. If not found, fetches from the database and stores in Redis.
        /// </summary>
        /// <returns>Returns a list of contacts, either from cache or database.</returns>
        [HttpGet("cache")]
        public async Task<ActionResult<object>> GetAllContactsFromCache()
        {
            string cacheKey = "AddressBookData";

           
            var cachedData = await _redisCacheService.GetAsync<List<AddressBookEntity>>(cacheKey);
            if (cachedData != null)
            {
                return Ok(new
                {
                    Message = "Data fetched from Redis Cache",
                    Source = "Redis",
                    Data = cachedData
                });
            }

           
            var contacts = _addressBookService.GetAllContacts();

            
            await _redisCacheService.SetAsync(cacheKey, contacts, TimeSpan.FromMinutes(10));

            return Ok(new
            {
                Message = "Data fetched from Database & stored in Redis",
                Source = "Database",
                Data = contacts
            });
        }




    }
}