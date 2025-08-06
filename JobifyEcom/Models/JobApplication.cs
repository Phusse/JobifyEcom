using System.ComponentModel.DataAnnotations;
using JobifyEcom.Enums;

namespace JobifyEcom.Models;

public class JobApplication
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid JobPostId { get; set; }
    public JobApplicationStatus Status { get; set; } = JobApplicationStatus.Pending;
    public DateTime DateRequested { get; set; } = DateTime.UtcNow;

    public User? Customer { get; set; }
    public JobPost? JobPost { get; set; }
}
