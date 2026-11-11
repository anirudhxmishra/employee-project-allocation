
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.ResourcePool;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Services.Implementations
{
    public class ResourcePoolService : IResourcePoolService
    {
        private readonly AppDbContext _context;

        public ResourcePoolService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ResourcePoolEmployeeDto>> GetAvailableEmployeesAsync(
            int? projectId = null)
        {
            // Validate project if supplied
            if (projectId.HasValue)
            {
                var projectExists = await _context.Projects
                    .AnyAsync(p => p.ProjectId == projectId.Value);

                if (!projectExists)
                    throw new KeyNotFoundException("Project not found.");
            }

            // Get employees who have no active allocation
            var query = _context.Employees
                .AsNoTracking()
                .Where(e => !_context.ProjectAllocations.Any(a =>
                    a.EmployeeCode == e.EmployeeCode &&
                    a.IsActive));

            // Match employees against all required project skills
            if (projectId.HasValue)
            {
                var requiredSkillIds = await _context.ProjectSkills
                    .Where(ps => ps.ProjectId == projectId.Value)
                    .Select(ps => ps.SkillId)
                    .ToListAsync();

                if (requiredSkillIds.Count > 0)
                {
                    query = query.Where(e =>
                        requiredSkillIds.All(requiredSkillId =>
                            e.EmployeeSkills.Any(es =>
                                es.SkillId == requiredSkillId)));
                }
            }

            return await query
                .OrderBy(e => e.FullName)
                .Select(e => new ResourcePoolEmployeeDto
                {
                    EmployeeCode = e.EmployeeCode,
                    FullName = e.FullName,
                    Email = e.Email,
                    Location = e.Location,
                    MobileNumber = e.MobileNumber,
                    TrainingName = e.TrainingName,
                    TrainingCompletedOn = e.TrainingCompletedOn,

                    Skills = e.EmployeeSkills
                        .Select(es => es.Skill.SkillName)
                        .OrderBy(name => name)
                        .ToList(),

                    ShortlistedProjects = _context.ProjectCandidates
                        .Where(pc => pc.EmployeeCode == e.EmployeeCode &&
                                     pc.Status != "Rejected")
                        .Select(pc => new ResourcePoolCandidateProjectDto
                        {
                            ProjectId = pc.ProjectId,
                            ProjectName = pc.Project.ProjectName,
                            Status = pc.Status
                        })
                        .ToList()
                })
                .ToListAsync();
        }
    }
}
