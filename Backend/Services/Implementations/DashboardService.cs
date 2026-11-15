using EmployeeProjectAllocation.Backend.Data;
using EmployeeProjectAllocation.Backend.DTOs.Dashboard;
using EmployeeProjectAllocation.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Backend.Services.Implementations
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
            // 1. Total employees
            var totalEmployees = await _context.Employees
                .AsNoTracking()
                .CountAsync();

            // 2. Employees with active allocations
            var allocatedEmployees = await _context.ProjectAllocations
                .AsNoTracking()
                .Where(a => a.IsActive)
                .Select(a => a.EmployeeCode)
                .Distinct()
                .CountAsync();

            // 3. Total projects
            var totalProjects = await _context.Projects
                .AsNoTracking()
                .CountAsync();

            // 4. Active projects
            var activeProjects = await _context.Projects
                .AsNoTracking()
                .CountAsync(p =>
                    p.Status.ToLower() == "active");

            // 5. Total allocations
            var totalAllocations = await _context.ProjectAllocations
                .AsNoTracking()
                .CountAsync();

            // 6. Active allocations
            var activeAllocations = await _context.ProjectAllocations
                .AsNoTracking()
                .CountAsync(a => a.IsActive);

            // Return dashboard response
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
