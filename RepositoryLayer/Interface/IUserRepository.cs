using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface IUserRepository
    {
        UserEntity AddUser(UserEntity user);
        UserEntity GetUserByEmail(string email);

        public void UpdateUser(UserEntity user);

        UserEntity GetUserByResetToken(string token);

    }
}
