using System.ComponentModel.DataAnnotations;

namespace CoreWebApp.Models
{
    public class Chatbot
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to Company
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;
    }
}