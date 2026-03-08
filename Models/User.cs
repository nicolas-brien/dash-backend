using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashBackend.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid NetworkId { get; set; }

        // navigation to the Network entity (FK is NetworkId)
        [ForeignKey(nameof(NetworkId))]
        public virtual Network Network { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = null!;

        // store a salted+hashed password (do not store plain text)
        [Required]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(200)]
        public string? FullName { get; set; }

        public bool IsActive { get; set; } = true;

        public string Role { get; set; } = "user";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}