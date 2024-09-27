using Core.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        [Required]
        [EmailAddress] 
        public string Email { get; set; } 
        public string Phone { get; set; }
        public string? DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public UserType UserType { get; set; } = UserType.User;
        public bool Status { get; set; }
        public DateTime? Created { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? Updated { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;
        public DateTime? Deleted { get; set; }
        public string? DeletedBy { get; set; } = string.Empty;
        public ICollection<UserWatchHistory> WatchHistories { get; set; } = new List<UserWatchHistory>();
    }
}
