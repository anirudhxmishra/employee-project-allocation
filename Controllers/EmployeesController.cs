
using EmployeeProjectAllocation.DTOs.Employees;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();

            return Ok(employees);
        }

        // GET: api/employees/{employeeCode}
        [HttpGet("{employeeCode}")]
        public async Task<ActionResult<EmployeeResponseDto>> GetEmployee(
            string employeeCode)
        {
            var employee = await _employeeService
                .GetEmployeeByCodeAsync(employeeCode);

            if (employee == null)
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee(
            CreateEmployeeDto dto)
        {
            try
            {
                var employee = await _employeeService
                    .CreateEmployeeAsync(dto);

                return CreatedAtAction(
                    nameof(GetEmployee),
                    new { employeeCode = employee.EmployeeCode },
                    employee
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/employees/{employeeCode}
        [HttpPut("{employeeCode}")]
        public async Task<IActionResult> UpdateEmployee(
            string employeeCode,
            UpdateEmployeeDto dto)
        {
            try
            {
                var updated = await _employeeService
                    .UpdateEmployeeAsync(employeeCode, dto);

                if (!updated)
                {
                    return NotFound(new
                    {
                        message = "Employee not found."
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE: api/employees/{employeeCode}
        [HttpDelete("{employeeCode}")]
        public async Task<IActionResult> DeleteEmployee(
            string employeeCode)
        {
            try
            {
                var deleted = await _employeeService
                    .DeleteEmployeeAsync(employeeCode);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Employee not found."
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
