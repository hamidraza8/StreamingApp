﻿using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public static class TokenHelper
    {
        public static string GenerateJwtToken(Guid userId, string name, string email, string phoneNumber, string usertype, int expiryTime, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey); //Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWT:Key"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (JwtRegisteredCustomClaimNames.Id, userId.ToString()),
                    new (JwtRegisteredCustomClaimNames.Name, name??string.Empty),
                    new (JwtRegisteredCustomClaimNames.EmailAddress, email??string.Empty),
                    new (JwtRegisteredCustomClaimNames.PhoneNumber, phoneNumber??string.Empty),
                    new (JwtRegisteredCustomClaimNames.UserType, usertype??string.Empty),

                }),
                Expires = DateTime.UtcNow.AddMinutes(expiryTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static (bool, string, string, string) ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            if (jsonToken.ValidTo < DateTime.UtcNow)
            {
                return (false, "Token has expired.", null, null);
            }
            var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "userid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return (false, "User ID not found in the token.", null, null);
            }
            var userType = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "usertype")?.Value;
            if (string.IsNullOrEmpty(userType))
            {
                return (false, "User ID not found in the token.", null, null);
            }

            return (true, string.Empty, userIdClaim, userType);
        }
        public static (bool, string, JObject) ValidateRefreshToken(string refreshToken, string secretKey)
        {
            var handler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            try
            {
                var key = Encoding.ASCII.GetBytes(secretKey);
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                handler.ValidateToken(refreshToken, tokenValidationParameters, out validatedToken);
            }
            catch (Exception)
            {
                return (false, "Invalid token.", null);
            }

            var jwtToken = validatedToken as JwtSecurityToken;

            if (jwtToken == null || jwtToken.ValidTo < DateTime.UtcNow)
            {
                return (false, "Token has expired.", null);
            }

            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            var name = jwtToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? string.Empty;
            var email = jwtToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? string.Empty;
            var phoneNumber = jwtToken.Claims.FirstOrDefault(x => x.Type == "phone")?.Value ?? string.Empty;
            var usertype = jwtToken.Claims.FirstOrDefault(x => x.Type == "usertype")?.Value ?? string.Empty;

            if (userId == null)
            {
                return (false, "Invalid token claims.", null);
            }

            var userDetails = new JObject
    {
        { "id", userId },
        { "name", name },
        { "email", email },
        { "phone", phoneNumber },
        { "usertype", usertype },
        //{ "ExpireTime", int.Parse(_configuration.GetValue<string>("JWT:RefreshExpireTime")) },
        //{ "refreshExpireTime", int.Parse(_configuration.GetValue<string>("JWT:ExpireTime")) },
        //{ "SecretKey", _configuration.GetValue<string>("JWT:Key")}
    };

            return (true, string.Empty, userDetails);
        }
    }
}
