using JobifyEcom.DTOs;
using JobifyEcom.Models;

public interface IJobService
{
    Task<JobPost> CreateJobAsync(Guid userId, CreateJobDto dto);
    Task<List<JobPost>> GetAllJobsAsync();
    Task<List<JobPost>> GetJobsByWorkerAsync(Guid workerId);
}
