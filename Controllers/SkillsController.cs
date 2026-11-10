
using EmployeeProjectAllocation.DTOs.Skills;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        // GET: api/Skills
        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _skillService.GetAllSkillsAsync();

            return Ok(skills);
        }

        // POST: api/Skills
        [HttpPost]
        public async Task<IActionResult> CreateSkill(CreateSkillDto dto)
        {
            try
            {
                var skill = await _skillService.CreateSkillAsync(dto);

                return CreatedAtAction(
                    nameof(GetAllSkills),
                    null,
                    skill
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // POST: api/Skills/assign/{employeeCode}
        [HttpPost("assign/{employeeCode}")]
        public async Task<IActionResult> AssignSkills(
            string employeeCode,
            AssignSkillsDto dto)
        {
            try
            {
                var assigned = await _skillService
                    .AssignSkillsAsync(employeeCode, dto);

                if (!assigned)
                {
                    return NotFound(new
                    {
                        message = "Employee not found."
                    });
                }

                return Ok(new
                {
                    message = "Skills assigned successfully.",
                    employeeCode,
                    assignedCount = dto.SkillIds.Distinct().Count()
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Skills/employee/{employeeCode}
        [HttpGet("employee/{employeeCode}")]
        public async Task<IActionResult> GetEmployeeSkills(
            string employeeCode)
        {
            try
            {
                var skills = await _skillService
                    .GetEmployeeSkillsAsync(employeeCode);

                return Ok(skills);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
