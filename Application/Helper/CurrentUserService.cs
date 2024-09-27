using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public struct JwtRegisteredCustomClaimNames
    {
        public const string Id = "id";
        public const string PhoneNumber = "phonenumber";
        public const string Name = "name";
        public const string EmailAddress = "emailaddress";
        public const string UserType = "usertype";
        public const string Role = "role";


    }
    public interface ICurrentUserService
    {
        string Id { get; }
        string Name { get; }
        string Email { get; }
        string PhoneNumber { get; }
        string UserType { get; }
        string Role { get; }
    }
    public class CurrentUserService(IHttpContextAccessor _httpContextAccessor) : ICurrentUserService
    {
        ClaimsPrincipal User => _httpContextAccessor?.HttpContext?.User ?? throw new UnauthorizedAccessException();
        public string Id => User?.FindFirst(JwtRegisteredCustomClaimNames.Id)?.Value
                            ?? string.Empty;
        public string Name => User?.FindFirst(JwtRegisteredCustomClaimNames.Name)?.Value
                              ?? string.Empty;
        public string Email => User?.FindFirst(JwtRegisteredCustomClaimNames.EmailAddress)?.Value
                               ?? string.Empty;
        public string PhoneNumber => User?.FindFirst(JwtRegisteredCustomClaimNames.PhoneNumber)?.Value
                                    ?? string.Empty;
        public string UserType => User?.FindFirst(JwtRegisteredCustomClaimNames.UserType)?.Value
                                   ?? string.Empty;
        public string Role => User?.FindFirst(JwtRegisteredCustomClaimNames.Role)?.Value
                                   ?? string.Empty;

    }
}
