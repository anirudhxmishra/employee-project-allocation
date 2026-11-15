using EmployeeProjectAllocation.Backend.DTOs.Hiring;

namespace EmployeeProjectAllocation.Backend.Services.Interfaces
{
    public interface IProjectHiringSummaryService
    {
        Task<ProjectHiringSummaryDto> GetHiringSummaryAsync(int projectId);
    }
}
