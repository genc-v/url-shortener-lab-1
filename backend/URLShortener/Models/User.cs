using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
namespace URLShortener.Models
{
    public class User
    {
        [Key] 
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string FullName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime? CreatedAt { get; set; }
        public bool isAdmin { get; set; }
    }
}

