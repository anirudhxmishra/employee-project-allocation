
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Dashboard
        [HttpGet]
        public async Task<IActionResult> GetDashboardSummary()
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

            var response = new DashboardResponseDto
            {
                TotalEmployees = totalEmployees,
                AllocatedEmployees = allocatedEmployees,
                AvailableEmployees = totalEmployees - allocatedEmployees,
                TotalProjects = totalProjects,
                ActiveProjects = activeProjects,
                TotalAllocations = totalAllocations,
                ActiveAllocations = activeAllocations
            };

            return Ok(response);
        }
    }
}
