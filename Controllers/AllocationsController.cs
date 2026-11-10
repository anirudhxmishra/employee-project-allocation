
using EmployeeProjectAllocation.DTOs.Allocations;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllocationsController : ControllerBase
    {
        private readonly IAllocationService _allocationService;

        public AllocationsController(IAllocationService allocationService)
        {
            _allocationService = allocationService;
        }

        // GET: api/Allocations
        [HttpGet]
        public async Task<IActionResult> GetAllAllocations()
        {
            var allocations =
                await _allocationService.GetAllAllocationsAsync();

            return Ok(allocations);
        }

        // GET: api/Allocations/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAllocationById(int id)
        {
            var allocation =
                await _allocationService.GetAllocationByIdAsync(id);

            if (allocation == null)
            {
                return NotFound(new
                {
                    message = "Allocation not found."
                });
            }

            return Ok(allocation);
        }

        // POST: api/Allocations
        [HttpPost]
        public async Task<IActionResult> CreateAllocation(
            CreateAllocationDto dto)
        {
            try
            {
                var allocation =
                    await _allocationService.CreateAllocationAsync(dto);

                return CreatedAtAction(
                    nameof(GetAllocationById),
                    new { id = allocation.AllocationId },
                    allocation
                );
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // GET: api/Allocations/employee/{employeeCode}
        [HttpGet("employee/{employeeCode}")]
        public async Task<IActionResult> GetEmployeeAllocationHistory(
            string employeeCode)
        {
            try
            {
                var history =
                    await _allocationService.GetEmployeeAllocationsAsync(employeeCode);

                return Ok(history);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/Allocations/release/{id}
        [HttpPut("release/{id:int}")]
        public async Task<IActionResult> ReleaseAllocation(
            int id,
            ReleaseAllocationDto dto)
        {
            try
            {
                var released =
                    await _allocationService.ReleaseAllocationAsync(id, dto);

                if (!released)
                {
                    return NotFound(new
                    {
                        message = "Allocation not found."
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
