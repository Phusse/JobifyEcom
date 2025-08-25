using JobifyEcom.DTOs;
using JobifyEcom.Models;

namespace JobifyEcom.Services;

public interface IJobService
{
    Task<JobPost> CreateJobAsync(Guid userId, CreateJobDto dto);
    Task<List<JobPost>> GetAllJobsAsync();
    Task<List<JobPost>> GetJobsByUserAsync(Guid workerId);
}
