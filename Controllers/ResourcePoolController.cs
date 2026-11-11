
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
