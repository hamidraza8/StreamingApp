using Core.DTOs.AuthDTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserRepository _userRepository) : ControllerBase
    {
        //this class will call UserRepository to fetch user and if user exists then it will return JWT token and if it fails then it will return 401. It will HtttpPost method and will take LoginDto as parameter.
        [HttpPost]
        //[EnableRateLimiting("Fixed")]
        public async Task<ActionResult<APIResponse>> Login(LoginDTO loginDto)
        {
            return await _userRepository.GetUserbyUserNameAndPassword(loginDto);
        }
    }
}
