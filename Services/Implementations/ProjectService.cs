
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Projects;
using EmployeeProjectAllocation.Models;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }

        // GET: All projects
        public async Task<List<ProjectResponseDto>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .AsNoTracking()
                .OrderBy(p => p.ProjectId)
                .Select(p => new ProjectResponseDto
                {
                    ProjectId = p.ProjectId,
                    ProjectCode = p.ProjectCode,
                    ProjectName = p.ProjectName,
                    ClientName = p.ClientName,
                    Technology = p.Technology,
                    Location = p.Location,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    RequiredEmployees = p.RequiredEmployees,
                    Status = p.Status
                })
                .ToListAsync();
        }

        // GET: Project by ID
        public async Task<ProjectResponseDto?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.ProjectId == id)
                .Select(p => new ProjectResponseDto
                {
                    ProjectId = p.ProjectId,
                    ProjectCode = p.ProjectCode,
                    ProjectName = p.ProjectName,
                    ClientName = p.ClientName,
                    Technology = p.Technology,
                    Location = p.Location,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    RequiredEmployees = p.RequiredEmployees,
                    Status = p.Status
                })
                .FirstOrDefaultAsync();
        }

        // POST: Create project
        public async Task<ProjectResponseDto> CreateProjectAsync(
            CreateProjectDto dto)
        {
            var projectCode = dto.ProjectCode.Trim();

            var exists = await _context.Projects
                .AnyAsync(p =>
                    p.ProjectCode.ToLower() == projectCode.ToLower());

            if (exists)
            {
                throw new InvalidOperationException(
                    "Project code already exists.");
            }

            ValidateProjectDates(dto.StartDate, dto.EndDate);

            var project = new Project
            {
                ProjectCode = projectCode,
                ProjectName = dto.ProjectName.Trim(),
                ClientName = dto.ClientName.Trim(),
                Technology = NormalizeOptionalString(dto.Technology),
                Location = NormalizeOptionalString(dto.Location),
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                RequiredEmployees = dto.RequiredEmployees,
                Status = "Active"
            };

            _context.Projects.Add(project);

            await _context.SaveChangesAsync();

            return MapToResponseDto(project);
        }

        // PUT: Update project
        public async Task<bool> UpdateProjectAsync(
            int id,
            UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
                return false;

            ValidateProjectDates(dto.StartDate, dto.EndDate);

            // Prevent reducing project capacity below active allocations
            var activeAllocationCount = await _context.ProjectAllocations
                .CountAsync(a => a.ProjectId == id && a.IsActive);

            if (dto.RequiredEmployees < activeAllocationCount)
            {
                throw new InvalidOperationException(
                    $"Required employees cannot be less than the current active allocation count ({activeAllocationCount}).");
            }

            var status = dto.Status.Trim();

            if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                status = "Active";
            }
            else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
            {
                status = "Inactive";
            }
            else if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                status = "Completed";
            }
            else
            {
                throw new InvalidOperationException(
                    "Invalid project status. Allowed values: Active, Inactive, Completed.");
            }

            project.ProjectName = dto.ProjectName.Trim();
            project.ClientName = dto.ClientName.Trim();
            project.Technology = NormalizeOptionalString(dto.Technology);
            project.Location = NormalizeOptionalString(dto.Location);
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.RequiredEmployees = dto.RequiredEmployees;
            project.Status = status;

            await _context.SaveChangesAsync();

            return true;
        }

        // DELETE: Delete project
        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
                return false;

            var hasAllocations = await _context.ProjectAllocations
                .AnyAsync(a => a.ProjectId == id);

            if (hasAllocations)
            {
                throw new InvalidOperationException(
                    "Cannot delete a project with allocation history.");
            }

            _context.Projects.Remove(project);

            await _context.SaveChangesAsync();

            return true;
        }

        // Validate project dates
        private static void ValidateProjectDates(
            DateTime? startDate,
            DateTime? endDate)
        {
            if (startDate.HasValue &&
                endDate.HasValue &&
                endDate.Value.Date < startDate.Value.Date)
            {
                throw new InvalidOperationException(
                    "Project end date cannot be earlier than start date.");
            }
        }

        // Normalize optional strings
        private static string? NormalizeOptionalString(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        // Map Project entity to response DTO
        private static ProjectResponseDto MapToResponseDto(Project project)
        {
            return new ProjectResponseDto
            {
                ProjectId = project.ProjectId,
                ProjectCode = project.ProjectCode,
                ProjectName = project.ProjectName,
                ClientName = project.ClientName,
                Technology = project.Technology,
                Location = project.Location,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                RequiredEmployees = project.RequiredEmployees,
                Status = project.Status
            };
        }
    }
}
