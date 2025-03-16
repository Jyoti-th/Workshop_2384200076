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

    

        // GET: Fetch all contacts
        [HttpGet]
        public ActionResult<List<AddressBookEntity>> GetAllContacts()
        {
            return _addressBookService.GetAllContacts(); // 🟢 Simple synchronous approach
        }

        //  GET: Get contact by ID
        [HttpGet("{id}")]
        public ActionResult<AddressBookEntity> GetContactById(int id)
        {
            var contact = _addressBookService.GetContactById(id);
            return contact;
           
        }

        // ADD: Add new Contact
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


        //  PUT: Update an existing contact
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


        //  DELETE: Remove a contact
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            var contact = _addressBookService.DeleteContact(id);
            return Ok(contact);
        }

        [HttpGet("cache")]
        public async Task<ActionResult<object>> GetAllContactsFromCache()
        {
            string cacheKey = "AddressBookData";

            // 🔹 Pehle Redis se try karo
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

            // 🔹 Agar Redis me nahi mila to DB se lo
            var contacts = _addressBookService.GetAllContacts();

            // 🔹 Fir Redis me store karo (expiry: 10 min)
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