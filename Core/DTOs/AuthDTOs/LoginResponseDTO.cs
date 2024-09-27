using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.AuthDTOs
{
    public class LoginResponseDTO
    {

            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public bool Status { get; set; }
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string ExpireTime { get; set; }
            public string RefreshExpireTime { get; set; }
        }
}
