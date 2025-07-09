using JobifyEcom.Models; 

public class JobRequest
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid JobPostId { get; set; }
    public string Status { get; set; } = "Pending"; // Options: Pending, Accepted, Rejected
    public DateTime DateRequested { get; set; } = DateTime.UtcNow;

}
