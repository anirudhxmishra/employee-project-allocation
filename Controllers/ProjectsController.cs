
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Projects;
using EmployeeProjectAllocation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _context.Projects
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
                    Status = p.Status
                })
                .ToListAsync();

            return Ok(projects);
        }

        // GET: api/Projects/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _context.Projects
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
                    Status = p.Status
                })
                .FirstOrDefaultAsync();

            if (project == null)
                return NotFound(new { message = "Project not found." });

            return Ok(project);
        }

        // POST: api/Projects
        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectDto dto)
        {
            var projectCode = dto.ProjectCode.Trim();

            var exists = await _context.Projects
                .AnyAsync(p => p.ProjectCode == projectCode);

            if (exists)
                return Conflict(new { message = "Project code already exists." });

            if (dto.StartDate.HasValue && dto.EndDate.HasValue &&
                dto.EndDate < dto.StartDate)
            {
                return BadRequest(new
                {
                    message = "End date cannot be earlier than start date."
                });
            }

            var project = new Project
            {
                ProjectCode = projectCode,
                ProjectName = dto.ProjectName.Trim(),
                ClientName = dto.ClientName.Trim(),
                Technology = dto.Technology,
                Location = dto.Location,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "Active"
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var response = new ProjectResponseDto
            {
                ProjectId = project.ProjectId,
                ProjectCode = project.ProjectCode,
                ProjectName = project.ProjectName,
                ClientName = project.ClientName,
                Technology = project.Technology,
                Location = project.Location,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status
            };

            return CreatedAtAction(
                nameof(GetProjectById),
                new { id = project.ProjectId },
                response
            );
        }

        // PUT: api/Projects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(
            int id,
            UpdateProjectDto dto)
        {
            var project = await _context.Projects
                .FindAsync(id);

            if (project == null)
                return NotFound(new { message = "Project not found." });

            if (dto.StartDate.HasValue && dto.EndDate.HasValue &&
                dto.EndDate < dto.StartDate)
            {
                return BadRequest(new
                {
                    message = "End date cannot be earlier than start date."
                });
            }

            project.ProjectName = dto.ProjectName.Trim();
            project.ClientName = dto.ClientName.Trim();
            project.Technology = dto.Technology;
            project.Location = dto.Location;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.Status = dto.Status.Trim();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Projects/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects
                .FindAsync(id);

            if (project == null)
                return NotFound(new { message = "Project not found." });

            var hasAllocations = await _context.ProjectAllocations
                .AnyAsync(a => a.ProjectId == id);

            if (hasAllocations)
            {
                return BadRequest(new
                {
                    message = "Cannot delete a project with allocation history."
                });
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
