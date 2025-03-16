using ModelLayer.DTO;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IUserService
    {
        UserEntity RegisterUser(UserDTO userDTO);

        UserEntity LoginUser(string email, string password);


        UserEntity GetUserByEmail(string email);
        void UpdateUser(UserEntity user);
        UserEntity GetUserByResetToken(string token);
    }

}
