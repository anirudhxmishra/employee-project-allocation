
using System.Data;
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.ProjectCandidates;
using EmployeeProjectAllocation.Models;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Services.Implementations
{
    public class ProjectCandidateService : IProjectCandidateService
    {
        private readonly AppDbContext _context;

        public ProjectCandidateService(AppDbContext context)
        {
            _context = context;
        }

        // POST: Shortlist an employee
        public async Task<ProjectCandidateResponseDto> CreateCandidateAsync(
            CreateProjectCandidateDto dto)
        {
            var employeeCode = dto.EmployeeCode.Trim();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == dto.ProjectId);

            if (project == null)
                throw new KeyNotFoundException("Project not found.");

            if (project.Status != "Active")
                throw new InvalidOperationException(
                    "Candidates can only be shortlisted for active projects.");

            var hasActiveAllocation = await _context.ProjectAllocations
                .AnyAsync(a =>
                    a.EmployeeCode == employeeCode &&
                    a.IsActive);

            if (hasActiveAllocation)
                throw new InvalidOperationException(
                    "Employee already has an active project allocation.");

            var alreadyExists = await _context.ProjectCandidates
                .AnyAsync(pc =>
                    pc.EmployeeCode == employeeCode &&
                    pc.ProjectId == dto.ProjectId);

            if (alreadyExists)
                throw new InvalidOperationException(
                    "Employee is already registered as a candidate for this project.");

            var candidate = new ProjectCandidate
            {
                EmployeeCode = employeeCode,
                ProjectId = dto.ProjectId,
                Status = "Shortlisted",
                ShortlistedOn = DateTime.UtcNow,
                Remarks = NormalizeOptionalString(dto.Remarks),
                Employee = employee,
                Project = project
            };

            _context.ProjectCandidates.Add(candidate);

            await _context.SaveChangesAsync();

            return MapToResponseDto(candidate);
        }

        // GET: All candidates for a project
        public async Task<List<ProjectCandidateResponseDto>>
            GetProjectCandidatesAsync(int projectId)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.ProjectId == projectId);

            if (!projectExists)
                throw new KeyNotFoundException("Project not found.");

            return await _context.ProjectCandidates
                .AsNoTracking()
                .Where(pc => pc.ProjectId == projectId)
                .OrderByDescending(pc => pc.ShortlistedOn)
                .Select(pc => new ProjectCandidateResponseDto
                {
                    CandidateId = pc.CandidateId,
                    EmployeeCode = pc.EmployeeCode,
                    EmployeeName = pc.Employee.FullName,
                    ProjectId = pc.ProjectId,
                    ProjectCode = pc.Project.ProjectCode,
                    ProjectName = pc.Project.ProjectName,
                    Status = pc.Status,
                    ShortlistedOn = pc.ShortlistedOn,
                    InterviewDate = pc.InterviewDate,
                    Remarks = pc.Remarks
                })
                .ToListAsync();
        }

        // PUT: Update status and automatically allocate when selected
        public async Task<ProjectCandidateResponseDto>
            UpdateCandidateStatusAsync(
                int candidateId,
                UpdateCandidateStatusDto dto,
                string allocatedBy)
        {
            var status = NormalizeStatus(dto.Status);

            if (status == "InterviewScheduled" &&
                !dto.InterviewDate.HasValue)
            {
                throw new InvalidOperationException(
                    "Interview date is required when scheduling an interview.");
            }

            if (status == "Selected" &&
                string.IsNullOrWhiteSpace(dto.Role))
            {
                throw new InvalidOperationException(
                    "Role is required when selecting a candidate.");
            }

            if (status == "Selected" &&
                string.IsNullOrWhiteSpace(allocatedBy))
            {
                throw new InvalidOperationException(
                    "AllocatedBy is required for automatic allocation.");
            }

            // Serializable transaction protects vacancy checks during selection
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var candidate = await _context.ProjectCandidates
                .Include(pc => pc.Employee)
                .Include(pc => pc.Project)
                .FirstOrDefaultAsync(pc => pc.CandidateId == candidateId);

            if (candidate == null)
                throw new KeyNotFoundException("Candidate not found.");

            if (candidate.Status == "Selected")
                throw new InvalidOperationException(
                    "This candidate has already been selected.");

            if (candidate.Project.Status != "Active")
                throw new InvalidOperationException(
                    "Candidates can only be selected for active projects.");

            if (status == "Selected")
            {
                var activeAllocationCount =
                    await _context.ProjectAllocations
                        .CountAsync(a =>
                            a.ProjectId == candidate.ProjectId &&
                            a.IsActive);

                if (activeAllocationCount >=
                    candidate.Project.RequiredEmployees)
                {
                    throw new InvalidOperationException(
                        "The project has no remaining vacancies.");
                }

                var employeeAlreadyAllocated =
                    await _context.ProjectAllocations
                        .AnyAsync(a =>
                            a.EmployeeCode == candidate.EmployeeCode &&
                            a.IsActive);

                if (employeeAlreadyAllocated)
                {
                    throw new InvalidOperationException(
                        "Employee already has an active project allocation.");
                }

                var allocation = new ProjectAllocation
                {
                    EmployeeCode = candidate.EmployeeCode,
                    ProjectId = candidate.ProjectId,
                    Role = dto.Role!.Trim(),
                    AllocationDate = DateTime.UtcNow,
                    IsActive = true,
                    AllocatedBy = allocatedBy.Trim()
                };

                _context.ProjectAllocations.Add(allocation);
            }

            candidate.Status = status;
            candidate.InterviewDate = dto.InterviewDate ?? candidate.InterviewDate;
            candidate.Remarks = NormalizeOptionalString(dto.Remarks);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return MapToResponseDto(candidate);
        }

        private static string NormalizeStatus(string value)
        {
            var status = value.Trim();

            var allowedStatuses = new[]
            {
                "Shortlisted",
                "InterviewScheduled",
                "Selected",
                "Rejected",
                "OnHold"
            };

            var matchedStatus = allowedStatuses.FirstOrDefault(
                s => s.Equals(
                    status,
                    StringComparison.OrdinalIgnoreCase));

            if (matchedStatus == null)
                throw new InvalidOperationException(
                    "Invalid candidate status.");

            return matchedStatus;
        }

        private static string? NormalizeOptionalString(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        private static ProjectCandidateResponseDto MapToResponseDto(
            ProjectCandidate candidate)
        {
            return new ProjectCandidateResponseDto
            {
                CandidateId = candidate.CandidateId,
                EmployeeCode = candidate.EmployeeCode,
                EmployeeName = candidate.Employee.FullName,
                ProjectId = candidate.ProjectId,
                ProjectCode = candidate.Project.ProjectCode,
                ProjectName = candidate.Project.ProjectName,
                Status = candidate.Status,
                ShortlistedOn = candidate.ShortlistedOn,
                InterviewDate = candidate.InterviewDate,
                Remarks = candidate.Remarks
            };
        }
    }
}
