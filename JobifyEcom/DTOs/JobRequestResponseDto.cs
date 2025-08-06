using JobifyEcom.Enums;

public class JobRequestResponseDto
{
    public Guid Id { get; set; }
    public JobApplicationStatus Status { get; set; }
    public DateTime DateRequested { get; set; }
    public Guid CustomerId { get; set; }
    public Guid JobPostId { get; set; }
}
