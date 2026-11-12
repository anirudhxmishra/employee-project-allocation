
using EmployeeProjectAllocation.DTOs.Hiring;

namespace EmployeeProjectAllocation.Services.Interfaces
{
    public interface IProjectHiringSummaryService
    {
        Task<ProjectHiringSummaryDto> GetHiringSummaryAsync(int projectId);
    }
}
