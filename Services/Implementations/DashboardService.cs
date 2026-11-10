
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Dashboard;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardResponseDto> GetDashboardAsync()
        {
            var totalEmployeesTask = _context.Employees
                .AsNoTracking()
                .CountAsync();

            var allocatedEmployeesTask = _context.ProjectAllocations
                .AsNoTracking()
                .Where(a => a.IsActive)
                .Select(a => a.EmployeeCode)
                .Distinct()
                .CountAsync();

            var totalProjectsTask = _context.Projects
                .AsNoTracking()
                .CountAsync();

            var activeProjectsTask = _context.Projects
                .AsNoTracking()
                .CountAsync(p =>
                    p.Status.ToLower() == "active");

            var totalAllocationsTask = _context.ProjectAllocations
                .AsNoTracking()
                .CountAsync();

            var activeAllocationsTask = _context.ProjectAllocations
                .AsNoTracking()
                .CountAsync(a => a.IsActive);

            await Task.WhenAll(
                totalEmployeesTask,
                allocatedEmployeesTask,
                totalProjectsTask,
                activeProjectsTask,
                totalAllocationsTask,
                activeAllocationsTask
            );

            var totalEmployees = totalEmployeesTask.Result;
            var allocatedEmployees = allocatedEmployeesTask.Result;

            return new DashboardResponseDto
            {
                TotalEmployees = totalEmployees,
                AllocatedEmployees = allocatedEmployees,
                AvailableEmployees = totalEmployees - allocatedEmployees,
                TotalProjects = totalProjectsTask.Result,
                ActiveProjects = activeProjectsTask.Result,
                TotalAllocations = totalAllocationsTask.Result,
                ActiveAllocations = activeAllocationsTask.Result
            };
        }
    }
}
