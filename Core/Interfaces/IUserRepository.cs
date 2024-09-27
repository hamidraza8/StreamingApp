using Core.DTOs.AuthDTOs;
using Core.DTOs.UserDTOs;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserRepository
    {
        public Task<APIResponse> GetAllUsers();
        public Task<APIResponse> GetUser(Guid id);
        public Task<APIResponse> GetUserbyUserNameAndPassword(LoginDTO  login);
        public Task<APIResponse> AddUser(UserRequestDTO user);
        public Task<APIResponse> UpdateUser(UserRequestDTO user);
        public Task<APIResponse> DeleteUser(Guid id);

    }
}
