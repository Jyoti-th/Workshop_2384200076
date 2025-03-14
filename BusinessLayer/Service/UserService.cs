using AutoMapper;
using BusinessLayer.Helpers;
using BusinessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public UserEntity RegisterUser(UserDTO userDTO)
        {
            // Check if user already exists
            var existingUser = _userRepository.GetUserByEmail(userDTO.Email);
            if (existingUser != null)
            {
                throw new Exception("User already exists!");
            }

            // Hash Password
            string hashedPassword = PasswordHasher.HashPassword(userDTO.Password);

            // Map DTO to Entity
            var userEntity = _mapper.Map<UserEntity>(userDTO);
            userEntity.PasswordHash = hashedPassword; // Save hashed password

            // Save user to database
            return _userRepository.AddUser(userEntity);
        }

        public UserEntity LoginUser(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password!");
            }

            return user;
        }

        public UserEntity GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmail(email);
        }

        public void UpdateUser(UserEntity user)
        {
            _userRepository.UpdateUser(user);
        }

        public UserEntity GetUserByResetToken(string token)
        {
            return _userRepository.GetUserByResetToken(token);
        }
    }
}