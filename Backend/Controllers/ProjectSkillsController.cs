using EmployeeProjectAllocation.Backend.DTOs.ProjectSkills;
using EmployeeProjectAllocation.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeProjectAllocation.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR Admin")]
    public class ProjectSkillsController : ControllerBase
    {
        private readonly IProjectSkillService _projectSkillService;

        public ProjectSkillsController(
            IProjectSkillService projectSkillService)
        {
            _projectSkillService = projectSkillService;
        }

        // GET: api/ProjectSkills/1
        [HttpGet("{projectId:int}")]
        public async Task<IActionResult> GetProjectSkills(int projectId)
        {
            var skills = await _projectSkillService
                .GetProjectSkillsAsync(projectId);

            return Ok(skills);
        }

        // POST: api/ProjectSkills/1
        [HttpPost("{projectId:int}")]
        public async Task<IActionResult> AssignProjectSkills(
            int projectId,
            [FromBody] AssignProjectSkillsDto dto)
        {
            var skills = await _projectSkillService
                .AssignProjectSkillsAsync(projectId, dto);

            return Ok(skills);
        }
    }
}
