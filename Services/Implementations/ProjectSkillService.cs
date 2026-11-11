
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.ProjectSkills;
using EmployeeProjectAllocation.Models;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Services.Implementations
{
    public class ProjectSkillService : IProjectSkillService
    {
        private readonly AppDbContext _context;

        public ProjectSkillService(AppDbContext context)
        {
            _context = context;
        }

        // GET: Skills required by a project
        public async Task<List<ProjectSkillResponseDto>> GetProjectSkillsAsync(
            int projectId)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.ProjectId == projectId);

            if (!projectExists)
                throw new KeyNotFoundException("Project not found.");

            return await _context.ProjectSkills
                .AsNoTracking()
                .Where(ps => ps.ProjectId == projectId)
                .Select(ps => new ProjectSkillResponseDto
                {
                    ProjectId = ps.ProjectId,
                    SkillId = ps.SkillId,
                    SkillName = ps.Skill.SkillName
                })
                .OrderBy(ps => ps.SkillName)
                .ToListAsync();
        }

        // POST: Assign skills to a project
        public async Task<List<ProjectSkillResponseDto>> AssignProjectSkillsAsync(
            int projectId,
            AssignProjectSkillsDto dto)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.ProjectId == projectId);

            if (!projectExists)
                throw new KeyNotFoundException("Project not found.");

            var skillIds = dto.SkillIds
                .Distinct()
                .ToList();

            var validSkillIds = await _context.Skills
                .Where(s => skillIds.Contains(s.SkillId))
                .Select(s => s.SkillId)
                .ToListAsync();

            if (validSkillIds.Count != skillIds.Count)
                throw new InvalidOperationException(
                    "One or more skill IDs are invalid.");

            var existingSkillIds = await _context.ProjectSkills
                .Where(ps => ps.ProjectId == projectId)
                .Select(ps => ps.SkillId)
                .ToListAsync();

            var newSkillIds = skillIds
                .Except(existingSkillIds)
                .ToList();

            foreach (var skillId in newSkillIds)
            {
                _context.ProjectSkills.Add(new ProjectSkill
                {
                    ProjectId = projectId,
                    SkillId = skillId
                });
            }

            await _context.SaveChangesAsync();

            return await GetProjectSkillsAsync(projectId);
        }
    }
}
