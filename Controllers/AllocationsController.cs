
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Allocations;
using EmployeeProjectAllocation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllocationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AllocationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Allocations
        [HttpGet]
        public async Task<IActionResult> GetAllAllocations()
        {
            var allocations = await _context.ProjectAllocations
                .Select(a => new AllocationResponseDto
                {
                    AllocationId = a.AllocationId,
                    EmployeeCode = a.EmployeeCode,
                    EmployeeName = a.Employee.FullName,
                    ProjectId = a.ProjectId,
                    ProjectName = a.Project.ProjectName,
                    Role = a.Role,
                    AllocationDate = a.AllocationDate,
                    ReleaseDate = a.ReleaseDate,
                    IsActive = a.IsActive,
                    AllocatedBy = a.AllocatedBy
                })
                .ToListAsync();

            return Ok(allocations);
        }

        // GET: api/Allocations/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllocationById(int id)
        {
            var allocation = await _context.ProjectAllocations
                .Where(a => a.AllocationId == id)
                .Select(a => new AllocationResponseDto
                {
                    AllocationId = a.AllocationId,
                    EmployeeCode = a.EmployeeCode,
                    EmployeeName = a.Employee.FullName,
                    ProjectId = a.ProjectId,
                    ProjectName = a.Project.ProjectName,
                    Role = a.Role,
                    AllocationDate = a.AllocationDate,
                    ReleaseDate = a.ReleaseDate,
                    IsActive = a.IsActive,
                    AllocatedBy = a.AllocatedBy
                })
                .FirstOrDefaultAsync();

            if (allocation == null)
                return NotFound(new { message = "Allocation not found." });

            return Ok(allocation);
        }

        // POST: api/Allocations
        [HttpPost]
        public async Task<IActionResult> CreateAllocation(
            CreateAllocationDto dto)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.EmployeeCode == dto.EmployeeCode);

            if (!employeeExists)
                return NotFound(new { message = "Employee not found." });

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == dto.ProjectId);

            if (project == null)
                return NotFound(new { message = "Project not found." });

            if (project.Status != "Active")
                return BadRequest(new { message = "Project is not active." });

            var alreadyAllocated = await _context.ProjectAllocations
                .AnyAsync(a =>
                    a.EmployeeCode == dto.EmployeeCode &&
                    a.IsActive);

            if (alreadyAllocated)
                return Conflict(new
                {
                    message = "Employee already has an active allocation."
                });

            var allocation = new ProjectAllocation
            {
                EmployeeCode = dto.EmployeeCode,
                ProjectId = dto.ProjectId,
                Role = dto.Role.Trim(),
                AllocationDate = DateTime.UtcNow,
                IsActive = true,
                AllocatedBy = dto.AllocatedBy.Trim()
            };

            _context.ProjectAllocations.Add(allocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAllocationById),
                new { id = allocation.AllocationId },
                new { message = "Employee allocated successfully." }
            );
        }

        // GET: api/Allocations/employee/{employeeCode}
        [HttpGet("employee/{employeeCode}")]
        public async Task<IActionResult> GetEmployeeAllocationHistory(
            string employeeCode)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.EmployeeCode == employeeCode);

            if (!employeeExists)
                return NotFound(new { message = "Employee not found." });

            var history = await _context.ProjectAllocations
                .Where(a => a.EmployeeCode == employeeCode)
                .OrderByDescending(a => a.AllocationDate)
                .Select(a => new AllocationResponseDto
                {
                    AllocationId = a.AllocationId,
                    EmployeeCode = a.EmployeeCode,
                    EmployeeName = a.Employee.FullName,
                    ProjectId = a.ProjectId,
                    ProjectName = a.Project.ProjectName,
                    Role = a.Role,
                    AllocationDate = a.AllocationDate,
                    ReleaseDate = a.ReleaseDate,
                    IsActive = a.IsActive,
                    AllocatedBy = a.AllocatedBy
                })
                .ToListAsync();

            return Ok(history);
        }

        // PUT: api/Allocations/release/{id}
        [HttpPut("release/{id}")]
        public async Task<IActionResult> ReleaseAllocation(
            int id,
            ReleaseAllocationDto dto)
        {
            var allocation = await _context.ProjectAllocations
                .FindAsync(id);

            if (allocation == null)
                return NotFound(new { message = "Allocation not found." });

            if (!allocation.IsActive)
                return BadRequest(new
                {
                    message = "This allocation has already been released."
                });

            if (dto.ReleaseDate.Date < allocation.AllocationDate.Date)
                return BadRequest(new
                {
                    message = "Release date cannot be earlier than allocation date."
                });

            allocation.ReleaseDate = dto.ReleaseDate;
            allocation.IsActive = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
