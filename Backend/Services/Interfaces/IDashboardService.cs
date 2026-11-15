using EmployeeProjectAllocation.Backend.DTOs.Dashboard;

namespace EmployeeProjectAllocation.Backend.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResponseDto> GetDashboardAsync();
    }
}