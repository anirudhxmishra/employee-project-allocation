using EmployeeProjectAllocation.DTOs.Dashboard;

namespace EmployeeProjectAllocation.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResponseDto> GetDashboardAsync();
    }
}