using EmployeeProjectAllocation.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeProjectAllocation.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR Admin")]
    public class ProjectHiringSummaryController : ControllerBase
    {
        private readonly IProjectHiringSummaryService _hiringSummaryService;

        public ProjectHiringSummaryController(
            IProjectHiringSummaryService hiringSummaryService)
        {
            _hiringSummaryService = hiringSummaryService;
        }

        // GET: api/ProjectHiringSummary/3
        [HttpGet("{projectId:int}")]
        public async Task<IActionResult> GetHiringSummary(int projectId)
        {
            var summary = await _hiringSummaryService
                .GetHiringSummaryAsync(projectId);

            return Ok(summary);
        }
    }
}
