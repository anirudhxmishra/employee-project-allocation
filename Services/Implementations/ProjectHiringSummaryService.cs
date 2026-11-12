
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Hiring;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Services.Implementations
{
    public class ProjectHiringSummaryService : IProjectHiringSummaryService
    {
        private readonly AppDbContext _context;

        public ProjectHiringSummaryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectHiringSummaryDto> GetHiringSummaryAsync(int projectId)
        {
            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null)
                throw new KeyNotFoundException("Project not found.");

            var activeAllocations = await _context.ProjectAllocations
                .CountAsync(a => a.ProjectId == projectId && a.IsActive);

            var candidates = _context.ProjectCandidates
                .Where(c => c.ProjectId == projectId);

            var shortlistedCount = await candidates
                .CountAsync(c => c.Status == "Shortlisted");

            var interviewScheduledCount = await candidates
                .CountAsync(c => c.Status == "InterviewScheduled");

            var selectedCount = await candidates
                .CountAsync(c => c.Status == "Selected");

            var rejectedCount = await candidates
                .CountAsync(c => c.Status == "Rejected");

            var onHoldCount = await candidates
                .CountAsync(c => c.Status == "OnHold");

            return new ProjectHiringSummaryDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                RequiredEmployees = project.RequiredEmployees,
                ActiveAllocations = activeAllocations,
                RemainingVacancies = Math.Max(
                    0, project.RequiredEmployees - activeAllocations),
                ShortlistedCount = shortlistedCount,
                InterviewScheduledCount = interviewScheduledCount,
                SelectedCount = selectedCount,
                RejectedCount = rejectedCount,
                OnHoldCount = onHoldCount
            };
        }
    }
}
