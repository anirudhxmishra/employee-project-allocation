using EmployeeProjectAllocation.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeProjectAllocation.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR Admin")]
    public class ResourcePoolController : ControllerBase
    {
        private readonly IResourcePoolService _resourcePoolService;

        public ResourcePoolController(
            IResourcePoolService resourcePoolService)
        {
            _resourcePoolService = resourcePoolService;
        }

        // GET: api/ResourcePool
        // GET: api/ResourcePool?projectId=1
        [HttpGet]
        public async Task<IActionResult> GetAvailableEmployees(
            [FromQuery] int? projectId)
        {
            var employees = await _resourcePoolService
                .GetAvailableEmployeesAsync(projectId);

            return Ok(employees);
        }
    }
}
