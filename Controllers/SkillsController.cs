using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Skills;
using EmployeeProjectAllocation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SkillsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Skills
        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _context.Skills
                .Select(s => new SkillResponseDto
                {
                    SkillId = s.SkillId,
                    SkillName = s.SkillName
                })
                .ToListAsync();

            return Ok(skills);
        }

        // POST: api/Skills
        [HttpPost]
        public async Task<IActionResult> CreateSkill(CreateSkillDto dto)
        {
            var skillName = dto.SkillName.Trim();

            var exists = await _context.Skills
                .AnyAsync(s => s.SkillName.ToLower() == skillName.ToLower());

            if (exists)
                return Conflict(new { message = "This skill already exists." });

            var skill = new Skill
            {
                SkillName = skillName
            };

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            var response = new SkillResponseDto
            {
                SkillId = skill.SkillId,
                SkillName = skill.SkillName
            };

            return CreatedAtAction(nameof(GetAllSkills), response);
        }
        // POST: api/Skills/assign/{employeeCode}
        [HttpPost("assign/{employeeCode}")]
        public async Task<IActionResult> AssignSkills(
            string employeeCode,
            AssignSkillsDto dto)
        {
            // Check whether employee exists
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.EmployeeCode == employeeCode);

            if (!employeeExists)
                return NotFound(new { message = "Employee not found." });

            // Remove duplicate skill IDs
            var skillIds = dto.SkillIds.Distinct().ToList();

            // Validate skill IDs
            var validSkillIds = await _context.Skills
                .Where(s => skillIds.Contains(s.SkillId))
                .Select(s => s.SkillId)
                .ToListAsync();

            if (validSkillIds.Count != skillIds.Count)
                return BadRequest(new { message = "One or more skill IDs are invalid." });

            // Get already assigned skills
            var existingSkillIds = await _context.EmployeeSkills
                .Where(es => es.EmployeeCode == employeeCode)
                .Select(es => es.SkillId)
                .ToListAsync();

            // Add only new assignments
            var newAssignments = skillIds
                .Except(existingSkillIds)
                .Select(skillId => new EmployeeSkill
                {
                    EmployeeCode = employeeCode,
                    SkillId = skillId
                })
                .ToList();

            _context.EmployeeSkills.AddRange(newAssignments);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Skills assigned successfully.",
                employeeCode,
                assignedCount = newAssignments.Count
            });
        }

        // GET: api/Skills/employee/{employeeCode}
        [HttpGet("employee/{employeeCode}")]
        public async Task<IActionResult> GetEmployeeSkills(string employeeCode)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.EmployeeCode == employeeCode);

            if (!employeeExists)
                return NotFound(new { message = "Employee not found." });

            var skills = await _context.EmployeeSkills
                .Where(es => es.EmployeeCode == employeeCode)
                .Select(es => new SkillResponseDto
                {
                    SkillId = es.Skill.SkillId,
                    SkillName = es.Skill.SkillName
                })
                .ToListAsync();

            return Ok(skills);
        }
    }
}