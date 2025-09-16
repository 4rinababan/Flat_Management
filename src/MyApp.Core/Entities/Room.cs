namespace MyApp.Core.Entities;

public class Room
{
    public int Id { get; set; }
    public string RoomNo { get; set; } = string.Empty;
    public int TotalOccupant { get; set; }
    public int Floor { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public string Details { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
