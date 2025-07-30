using System.ComponentModel.DataAnnotations;

namespace CoreWebApp.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Chatbot> Chatbots { get; set; } = new List<Chatbot>();
        public virtual ICollection<LLM> LLMs { get; set; } = new List<LLM>();
    }
}