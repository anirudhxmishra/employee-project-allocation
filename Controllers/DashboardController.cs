using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: api/Dashboard
        [HttpGet]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var response = await _dashboardService.GetDashboardAsync();

            return Ok(response);
        }
    }
}