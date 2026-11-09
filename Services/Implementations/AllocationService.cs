
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Allocations;
using EmployeeProjectAllocation.Models;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Services.Implementations
{
    public class AllocationService : IAllocationService
    {
        private readonly AppDbContext _context;

        public AllocationService(AppDbContext context)
        {
            _context = context;
        }

        // GET: All allocations
        public async Task<List<AllocationResponseDto>> GetAllAllocationsAsync()
        {
            return await _context.ProjectAllocations
                .AsNoTracking()
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
        }

        // GET: Allocation by ID
        public async Task<AllocationResponseDto?> GetAllocationByIdAsync(int id)
        {
            return await _context.ProjectAllocations
                .AsNoTracking()
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
        }

        // POST: Create allocation
        public async Task<AllocationResponseDto> CreateAllocationAsync(
            CreateAllocationDto dto)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeCode == dto.EmployeeCode);

            if (employee == null)
            {
                throw new KeyNotFoundException("Employee not found.");
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == dto.ProjectId);

            if (project == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            if (!string.Equals(
                project.Status,
                "Active",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Cannot allocate an employee to an inactive project.");
            }

            var alreadyAllocated = await _context.ProjectAllocations
                .AnyAsync(a =>
                    a.EmployeeCode == dto.EmployeeCode &&
                    a.IsActive);

            if (alreadyAllocated)
            {
                throw new InvalidOperationException(
                    "Employee already has an active project allocation.");
            }

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

            return new AllocationResponseDto
            {
                AllocationId = allocation.AllocationId,
                EmployeeCode = allocation.EmployeeCode,
                EmployeeName = employee.FullName,
                ProjectId = allocation.ProjectId,
                ProjectName = project.ProjectName,
                Role = allocation.Role,
                AllocationDate = allocation.AllocationDate,
                ReleaseDate = allocation.ReleaseDate,
                IsActive = allocation.IsActive,
                AllocatedBy = allocation.AllocatedBy
            };
        }

        // GET: Employee allocation history
        public async Task<List<AllocationResponseDto>> GetEmployeeAllocationsAsync(
            string employeeCode)
        {
            return await _context.ProjectAllocations
                .AsNoTracking()
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
        }

        // PUT: Release allocation
        public async Task<bool> ReleaseAllocationAsync(
            int id,
            ReleaseAllocationDto dto)
        {
            var allocation = await _context.ProjectAllocations
                .FindAsync(id);

            if (allocation == null)
                return false;

            if (!allocation.IsActive)
            {
                throw new InvalidOperationException(
                    "This allocation has already been released.");
            }

            if (dto.ReleaseDate.Date < allocation.AllocationDate.Date)
            {
                throw new InvalidOperationException(
                    "Release date cannot be earlier than allocation date.");
            }

            allocation.ReleaseDate = dto.ReleaseDate;
            allocation.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
