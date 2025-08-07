using JobifyEcom.DTOs;
using JobifyEcom.Models;

namespace JobifyEcom.Services;

public interface IWorkerService
{
    Task<WorkerProfile> CreateProfileAsync(Guid userId, CreateWorkerProfileDto dto);
    Task<WorkerProfile?> GetMyProfileAsync(Guid userId);

}