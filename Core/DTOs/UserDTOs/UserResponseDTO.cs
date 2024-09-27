using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UserDTOs
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string? Role { get; set; }
        public UserType UserType { get; set; } = UserType.User;
        public bool Status { get; set; }
        public DateTime? Created { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? Updated { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;
        public DateTime? Deleted { get; set; }
        public string? DeletedBy { get; set; } = string.Empty;
    }
}
