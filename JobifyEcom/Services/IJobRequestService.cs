using JobifyEcom.DTOs;
using JobifyEcom.Enums;

namespace JobifyEcom.Services;

public interface IJobApplicationService
{
    Task<JobRequestResponseDto> CreateApplicationAsync(RequestJobDto dto);
    Task<JobRequestResponseDto> GetByIdAsync(Guid id);
    Task<bool> UpdateStatusAsync(Guid id, JobApplicationStatus status);
}