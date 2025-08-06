using JobifyEcom.Enums;

public interface IJobApplicationService
{
    Task<JobRequestResponseDto> CreateApplicationAsync(RequestJobDto dto);
    Task<JobRequestResponseDto> GetByIdAsync(Guid id);
    Task<bool> UpdateStatusAsync(Guid id, JobApplicationStatus status);
}