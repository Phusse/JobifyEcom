namespace JobifyEcom.DTOs;

public class WorkerProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public int JobsCompleted { get; set; }
    public double? AverageRating { get; set; }
    public DateTime CreatedAt { get; set; }
}
