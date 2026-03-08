using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashBackend.Models
{
    /// <summary>
    /// Represents a company / network in the system.
    /// </summary>
    public class Network
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Address { get; set; }

        [MaxLength(200), Url]
        public string? Website { get; set; }

        [MaxLength(200), EmailAddress]
        public string? ContactEmail { get; set; }

        [MaxLength(50), Phone]
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Concurrency token for EF Core
        [Timestamp]
        public byte[] RowVersion { get; set; }

        // Navigation: users/employees belonging to this network (optional)
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}