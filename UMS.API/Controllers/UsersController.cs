using Application.Services;
using Core.DTOs.UserDTOs;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(ICommonService commonService) : ControllerBase
    {
        //[Authorize]
        [HttpGet("get-all-users")]
        public async Task<ActionResult<APIResponse>> GetAllUser()
        {
            var response = await commonService.GetAllUsers();
            return response;
        }
        [HttpGet("get-user-by-id/{id}")]
       // [Authorize]
        public async Task<ActionResult<APIResponse>> GetUser(Guid id)
        {
            var response = await commonService.GetUser(id);
            return response;
        }

        [HttpPut("update-user")]
       // [Authorize]
        public async Task<ActionResult<APIResponse>> PutUser(UserRequestDTO user)
        {
            var response = await commonService.UpdateUser(user);
            return response;
        }

        [HttpPost("add-user")]
        //[Authorize]
        public async Task<ActionResult<APIResponse>> AddUser(UserRequestDTO user)
        {

            var response = await commonService.AddUser(user);
            return response;
        }

        [HttpDelete("delete-user/{id}")]
        //[Authorize]
        public async Task<ActionResult<APIResponse>> DeleteEmployee(Guid id)
        {
            var response = await commonService.DeleteUser(id);
            return response;
        }

    }
}
