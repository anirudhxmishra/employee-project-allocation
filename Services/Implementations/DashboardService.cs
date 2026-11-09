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
            var totalEmployees = await _context.Employees.CountAsync();

            var allocatedEmployees = await _context.ProjectAllocations
                .Where(a => a.IsActive)
                .Select(a => a.EmployeeCode)
                .Distinct()
                .CountAsync();

            var totalProjects = await _context.Projects.CountAsync();

            var activeProjects = await _context.Projects
                .CountAsync(p => p.Status == "Active");

            var totalAllocations = await _context.ProjectAllocations.CountAsync();

            var activeAllocations = await _context.ProjectAllocations
                .CountAsync(a => a.IsActive);

            return new DashboardResponseDto
            {
                TotalEmployees = totalEmployees,
                AllocatedEmployees = allocatedEmployees,
                AvailableEmployees = totalEmployees - allocatedEmployees,
                TotalProjects = totalProjects,
                ActiveProjects = activeProjects,
                TotalAllocations = totalAllocations,
                ActiveAllocations = activeAllocations
            };
        }
    }
}