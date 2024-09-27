using Core.DTOs.UserDTOs;
using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICommonService
    {
        //public Task<APIResponse> SupportMessage(SupportRequestDTO request);
        public Task<APIResponse> GetAllUsers();
        public Task<APIResponse> GetUser(Guid Id);
        public Task<APIResponse> AddUser(UserRequestDTO Id);
        public Task<APIResponse> UpdateUser(UserRequestDTO Id);
        public Task<APIResponse> DeleteUser(Guid Id);

    }
    public class UserService(IUserRepository userRepository) : ICommonService
    {
        public Task<APIResponse> GetAllUsers()
        {
                return userRepository.GetAllUsers();
                
        }

        public Task<APIResponse> GetUser(Guid Id)
        {
            return userRepository.GetUser(Id);
        }

        Task<APIResponse> ICommonService.AddUser(UserRequestDTO user)
        {

            return userRepository.AddUser(user);
        }

        Task<APIResponse> ICommonService.DeleteUser(Guid Id)
        {
            return userRepository.DeleteUser(Id);
        }

        Task<APIResponse> ICommonService.UpdateUser(UserRequestDTO user)
        {
            return userRepository.UpdateUser(user);
        }
    }
}
