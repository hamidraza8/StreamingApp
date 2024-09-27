using Application.Helper;
using AutoMapper;
using Core.DTOs.AuthDTOs;
using Core.DTOs.UserDTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class UserRepository(UMSDbContext _context, IMapper _mapper, ICurrentUserService currentUser,IHelpers helpers, IConfiguration _configuration) : IUserRepository
    {
        public async Task<APIResponse> GetAllUsers()
        {
            var users = await _context.Users
    .Select(u => new UserResponseDTO
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Phone = u.Phone,
        Status = u.Status,
        DateOfBirth = u.DateOfBirth,
        Gender = u.Gender,
        Role = u.Role,
        UserType = u.UserType,
        Created = u.Created,
        CreatedBy = u.CreatedBy,
        Updated = u.Updated,
        UpdatedBy = u.UpdatedBy,
        Deleted = u.Deleted,
        DeletedBy = u.DeletedBy,
    })
    .ToListAsync();

            return new APIResponse
            {
                ApiCode = 0,
                DisplayMessage = "Success",
                Data = users
            };

        }

        public async Task<APIResponse> GetUser(Guid id)
        {
            var user = await _context.Users
     .Select(u => new UserResponseDTO
     {
         Id = u.Id,
         Name = u.Name,
         Email = u.Email,
         Phone = u.Phone,
         Status = u.Status,
         DateOfBirth = u.DateOfBirth,
         Gender = u.Gender,
         Role = u.Role,
         UserType = u.UserType,
         Created = u.Created,
         CreatedBy = u.CreatedBy,
         Updated = u.Updated,
         UpdatedBy = u.UpdatedBy,
         Deleted = u.Deleted,
         DeletedBy = u.DeletedBy,
     })
     .ToListAsync();
            if (user == null)
            {
                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "User Not Found",
                    Data = null
                };
            }
            return new APIResponse
            {
                ApiCode = 0,
                DisplayMessage = "User Retrieved Successfully",
                Data = user
            };
        }

        public async Task<APIResponse> AddUser(UserRequestDTO user)
        {
            try
            {
                var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower() ||
                                           u.Phone == user.Phone);
                if (existingUser != null)
                {
                    if (existingUser.Email.ToLower() == user.Email.ToLower())
                    {
                        return new APIResponse
                        {
                            ApiCode = 99,
                            DisplayMessage = "Email already exists.",
                            DisplayCode = "400002",
                            Data = null
                        };
                    }
                    else if (existingUser.Phone == user.Phone)
                    {
                        return new APIResponse
                        {
                            ApiCode = 99,
                            DisplayMessage = "Phone number already exists.",
                            DisplayCode = "400003",
                            Data = null
                        };
                    }

                }
                var mapuser = _mapper.Map<User>(user);
                mapuser.Id = Guid.NewGuid();
                mapuser.Password= helpers.HashPassword(user.Password);
                mapuser.Created = DateTime.UtcNow;
                mapuser.CreatedBy = currentUser.Name;
                mapuser.Updated = DateTime.UtcNow;
                mapuser.UpdatedBy = currentUser.Name;
                await _context.Users.AddAsync(mapuser);
                await _context.SaveChangesAsync();
                var useradded = await _context.Users
                .Where(u => u.Id == mapuser.Id)
                    .Select(u => new UserResponseDTO
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        Phone = u.Phone,
                        Status = u.Status,
                        DateOfBirth = u.DateOfBirth,
                        Gender = u.Gender,
                        Role = u.Role,
                        UserType = u.UserType,
                        Created = u.Created,
                        CreatedBy = u.CreatedBy,
                        Updated = u.Updated,
                        UpdatedBy = u.UpdatedBy,
                        Deleted = u.Deleted,
                        DeletedBy = u.DeletedBy,
                    }).FirstOrDefaultAsync();
                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "User added successfully.",
                    Data = useradded
                };
            }
            catch (Exception ex)
            {

                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Error while Adding user.",
                    Data = ex.Message
                };
            }
        }

        public async Task<APIResponse> DeleteUser(Guid id)
        {
            try
            {
                var user = await _context.Users
                                       .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return new APIResponse
                    {
                        ApiCode = 99,
                        DisplayMessage = "User not found.",
                        Data = null
                    };
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "User Deleted Successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {

                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Error while deleting user.",
                    Data = ex.Message
                };
            }
        }

        public async Task<APIResponse> UpdateUser(UserRequestDTO user)
        {
            var existingUser = await _context.Users
         .FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "User not found.",
                    Data = null
                };
            }
            var alreadyexistparam = await _context.Users
                .FirstOrDefaultAsync(u => u.Id != user.Id && u.Email.ToLower().Equals(user.Email.ToLower()) ||
                u.Id != user.Id && u.Phone == user.Phone);
            if (alreadyexistparam != null && alreadyexistparam.Email.ToLower() == user.Email.ToLower())
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Email already exists.",
                    DisplayCode = "400002",
                    Data = null
                };
            }
            else if (alreadyexistparam != null && alreadyexistparam.Phone == user.Phone)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Phone number already exists.",
                    DisplayCode = "400003",
                    Data = null
                };
            }
            existingUser.Updated = DateTime.UtcNow;
            existingUser.UpdatedBy = currentUser.Name;
            var newUser = _mapper.Map(user, existingUser);
            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new APIResponse
            {
                ApiCode = 0,
                DisplayMessage = "User Updated Successfully",
                Data = existingUser
            };
        }
        public async Task<APIResponse> GetUserbyUserNameAndPassword(LoginDTO login)
        {
            var user = new User();
            user = await _context.Users.AsNoTracking()
                   .Where(u => u.Email.ToLower() == login.UserName.ToLower())
                   .FirstOrDefaultAsync();

            if (user == null)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "User not Found",
                    DisplayCode = "400037",
                    Data = null
                };
            }
            else if (user.Status != true)
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "User Not Active",
                    DisplayCode = "400039",
                    Data = null
                };
            }
           
            else if (!helpers.VerifyPassword(login.Password, user.Password))
            {
                return new APIResponse
                {
                    ApiCode = 99,
                    DisplayMessage = "Invalid Password",
                    Data = null
                };
            }
            else
            {

                var token = TokenHelper.GenerateJwtToken(user.Id, user.Name, user.Email,  user.Phone, user.UserType.ToString(), _configuration.GetValue<int>("JWT:ExpireTime"), _configuration.GetValue<string>("JWT:Key"));
                var refreshToken = TokenHelper.GenerateJwtToken(user.Id, user.Name, user.Email, user.Phone, user.UserType.ToString(), _configuration.GetValue<int>("JWT:RefreshExpireTime"), _configuration.GetValue<string>("JWT:Key"));
                return new APIResponse
                {
                    ApiCode = 0,
                    DisplayMessage = "User retrieved successfully.",
                    Data = new LoginResponseDTO
                    {
                        Id=user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        Phone = user.Phone,
                        Status = user.Status,
                        Token = token,
                        RefreshToken = refreshToken,
                        ExpireTime = _configuration.GetValue<string>("JWT:ExpireTime"),
                        RefreshExpireTime = _configuration.GetValue<string>("JWT:RefreshExpireTime")
                    }
                };
            }
        }
    }
}
