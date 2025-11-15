using System.ComponentModel.DataAnnotations;

namespace MyApp.Core.Entities
{
    public class Card
    {
        public int Id { get; set; }
        public string CardNo { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
