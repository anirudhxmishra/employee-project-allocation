
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Skills;
using EmployeeProjectAllocation.Models;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Services.Implementations
{
    public class SkillService : ISkillService
    {
        private readonly AppDbContext _context;

        public SkillService(AppDbContext context)
        {
            _context = context;
        }

        // Get all available skills
        public async Task<List<SkillResponseDto>> GetAllSkillsAsync()
        {
            return await _context.Skills
                .AsNoTracking()
                .Select(s => new SkillResponseDto
                {
                    SkillId = s.SkillId,
                    SkillName = s.SkillName
                })
                .ToListAsync();
        }

        // Create a new skill
        public async Task<SkillResponseDto> CreateSkillAsync(
            CreateSkillDto dto)
        {
            var skillName = dto.SkillName.Trim();

            var skillExists = await _context.Skills
                .AnyAsync(s =>
                    s.SkillName.ToLower() == skillName.ToLower());

            if (skillExists)
            {
                throw new InvalidOperationException(
                    "Skill already exists.");
            }

            var skill = new Skill
            {
                SkillName = skillName
            };

            _context.Skills.Add(skill);

            await _context.SaveChangesAsync();

            return new SkillResponseDto
            {
                SkillId = skill.SkillId,
                SkillName = skill.SkillName
            };
        }

        // Assign multiple skills to an employee
        public async Task<bool> AssignSkillsAsync(
            string employeeCode,
            AssignSkillsDto dto)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.EmployeeCode == employeeCode);

            if (!employeeExists)
                return false;

            var skillIds = dto.SkillIds.Distinct().ToList();

            var existingSkills = await _context.Skills
                .Where(s => skillIds.Contains(s.SkillId))
                .Select(s => s.SkillId)
                .ToListAsync();

            if (existingSkills.Count != skillIds.Count)
            {
                throw new InvalidOperationException(
                    "One or more skill IDs are invalid.");
            }

            var alreadyAssigned = await _context.EmployeeSkills
                .Where(es =>
                    es.EmployeeCode == employeeCode &&
                    skillIds.Contains(es.SkillId))
                .Select(es => es.SkillId)
                .ToListAsync();

            var newAssignments = skillIds
                .Except(alreadyAssigned)
                .Select(skillId => new EmployeeSkill
                {
                    EmployeeCode = employeeCode,
                    SkillId = skillId
                })
                .ToList();

            if (newAssignments.Count > 0)
            {
                _context.EmployeeSkills.AddRange(newAssignments);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        // Get skills assigned to an employee
        public async Task<List<SkillResponseDto>> GetEmployeeSkillsAsync(
            string employeeCode)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.EmployeeCode == employeeCode);

            if (!employeeExists)
            {
                throw new KeyNotFoundException(
                    $"Employee with code '{employeeCode}' was not found.");
            }

            return await _context.EmployeeSkills
                .AsNoTracking()
                .Where(es => es.EmployeeCode == employeeCode)
                .Select(es => new SkillResponseDto
                {
                    SkillId = es.Skill.SkillId,
                    SkillName = es.Skill.SkillName
                })
                .ToListAsync();
        }
    }
}
