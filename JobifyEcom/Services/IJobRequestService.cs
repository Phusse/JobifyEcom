public interface IJobRequestService
{
    Task<JobRequestResponseDto> CreateRequestAsync(RequestJobDto dto);
    Task<JobRequestResponseDto> GetByIdAsync(Guid id);
    Task<bool> UpdateStatusAsync(Guid id, string status);
}