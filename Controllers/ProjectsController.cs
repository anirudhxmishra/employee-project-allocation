
using EmployeeProjectAllocation.DTOs.Projects;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();

            return Ok(projects);
        }

        // GET: api/Projects/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
                return NotFound(new { message = "Project not found." });

            return Ok(project);
        }

        // POST: api/Projects
        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectDto dto)
        {
            try
            {
                var project = await _projectService.CreateProjectAsync(dto);

                return CreatedAtAction(
                    nameof(GetProjectById),
                    new { id = project.ProjectId },
                    project
                );
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "Project code already exists.")
                    return Conflict(new { message = ex.Message });

                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Projects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(
            int id,
            UpdateProjectDto dto)
        {
            try
            {
                var updated = await _projectService.UpdateProjectAsync(id, dto);

                if (!updated)
                    return NotFound(new { message = "Project not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Projects/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var deleted = await _projectService.DeleteProjectAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Project not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
