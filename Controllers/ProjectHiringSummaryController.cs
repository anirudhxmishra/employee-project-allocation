
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
