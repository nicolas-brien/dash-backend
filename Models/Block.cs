using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashBackend.Models
{
    public class Block
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid DashId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Text { get; set; } = null!;

        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }
}