using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

public class JobPost
{
    [Key] public Guid Id { get; set; }
    public Guid WorkerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    [Required] public JobStatus Status { get; set; } = JobStatus.Available;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public WorkerProfile? Worker { get; set; }
}
