using JobifyEcom.DTOs;
using JobifyEcom.Models;

namespace JobifyEcom.Services;

public interface IJobService
{
    Task<Job> CreateJobAsync(Guid userId, CreateJobDto dto);
    Task<List<Job>> GetAllJobsAsync();
    Task<List<Job>> GetJobsByUserAsync(Guid workerId);
}
