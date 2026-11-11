
using EmployeeProjectAllocation.DTOs.ProjectCandidates;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectCandidatesController : ControllerBase
    {
        private readonly IProjectCandidateService _candidateService;

        public ProjectCandidatesController(
            IProjectCandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        // POST: api/ProjectCandidates
        // Shortlist an employee
        [HttpPost]
        public async Task<IActionResult> CreateCandidate(
            [FromBody] CreateProjectCandidateDto dto)
        {
            var candidate = await _candidateService
                .CreateCandidateAsync(dto);

            return CreatedAtAction(
                nameof(GetProjectCandidates),
                new { projectId = candidate.ProjectId },
                candidate);
        }

        // GET: api/ProjectCandidates/project/1
        // Get all candidates for a project
        [HttpGet("project/{projectId:int}")]
        public async Task<IActionResult> GetProjectCandidates(
            int projectId)
        {
            var candidates = await _candidateService
                .GetProjectCandidatesAsync(projectId);

            return Ok(candidates);
        }

        // PUT: api/ProjectCandidates/1/status
        // Update candidate status and handle automatic allocation
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateCandidateStatus(
            int id,
            [FromBody] UpdateCandidateStatusDto dto)
        {
            // Temporary identity until JWT authentication is implemented.
            var allocatedBy = User.Identity?.Name ?? "System";

            var candidate = await _candidateService
                .UpdateCandidateStatusAsync(id, dto, allocatedBy);

            return Ok(candidate);
        }
    }
}
