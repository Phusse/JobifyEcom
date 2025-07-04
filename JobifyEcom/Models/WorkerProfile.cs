using System.ComponentModel.DataAnnotations;

namespace JobifyEcom.Models;

public class WorkerProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }  
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public double Rating { get; set; } = 0.0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User? User { get; set; }
}
