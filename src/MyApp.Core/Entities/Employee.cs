namespace MyApp.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NRP { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }

        // Foreign keys
        public string Status { get; set; } = string.Empty;
        public int RankId { get; set; }
        public Rank Rank { get; set; } = null!;
        public int PositionId { get; set; }
        public Position Position { get; set; } = null!;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public DateOnly JoinDate { get; set; }
        public byte[]? PhotoData { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}