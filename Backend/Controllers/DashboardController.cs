using EmployeeProjectAllocation.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeProjectAllocation.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR Admin")]
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