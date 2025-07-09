using JobifyEcom.Data;
using JobifyEcom.Models; 


public class JobRequestService : IJobRequestService
{
    private readonly AppDbContext _context;

    public JobRequestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<JobRequestResponseDto> CreateRequestAsync(RequestJobDto dto)
    {
        var request = new JobRequest
        {
            Id = Guid.NewGuid(),
            CustomerId = dto.CustomerId,
            JobPostId = dto.JobPostId,
            Status = "Pending",
            DateRequested = DateTime.UtcNow
        };

        _context.JobRequests.Add(request);
        await _context.SaveChangesAsync();

        return new JobRequestResponseDto
        {
            Id = request.Id,
            CustomerId = request.CustomerId,
            JobPostId = request.JobPostId,
            Status = request.Status,
            DateRequested = request.DateRequested
        };
    }

    public async Task<JobRequestResponseDto> GetByIdAsync(Guid id)
    {
        var request = await _context.JobRequests.FindAsync(id);
        if (request == null) return null;

        return new JobRequestResponseDto
        {
            Id = request.Id,
            CustomerId = request.CustomerId,
            JobPostId = request.JobPostId,
            Status = request.Status,
            DateRequested = request.DateRequested
        };
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string status)
    {
        var request = await _context.JobRequests.FindAsync(id);
        if (request == null) return false;

        request.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }
}
