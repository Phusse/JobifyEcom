using JobifyEcom.DTOs;
using JobifyEcom.Models;

namespace JobifyEcom.Services;

public interface IWorkerService
{
    Task<Worker> CreateProfileAsync(Guid userId, CreateWorkerProfileDto dto);
    Task<Worker?> GetMyProfileAsync(Guid userId);

}