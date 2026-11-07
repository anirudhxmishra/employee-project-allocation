using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Employees;
using EmployeeProjectAllocation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployees()
        {
            var employees = await _context.Employees
                .Select(e => new EmployeeResponseDto
                {
                    EmployeeCode = e.EmployeeCode,
                    FullName = e.FullName,
                    Email = e.Email,
                    Location = e.Location,
                    MobileNumber = e.MobileNumber,
                    TrainingName = e.TrainingName,
                    TrainingCompletedOn = e.TrainingCompletedOn
                })
                .ToListAsync();

            return Ok(employees);
        }

        // GET: api/employees/85010193
        [HttpGet("{employeeCode}")]
        public async Task<ActionResult<EmployeeResponseDto>> GetEmployee(string employeeCode)
        {
            var employee = await _context.Employees
                .Where(e => e.EmployeeCode == employeeCode)
                .Select(e => new EmployeeResponseDto
                {
                    EmployeeCode = e.EmployeeCode,
                    FullName = e.FullName,
                    Email = e.Email,
                    Location = e.Location,
                    MobileNumber = e.MobileNumber,
                    TrainingName = e.TrainingName,
                    TrainingCompletedOn = e.TrainingCompletedOn
                })
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee(
            CreateEmployeeDto dto)
        {
            if (await _context.Employees.AnyAsync(
                e => e.EmployeeCode == dto.EmployeeCode))
            {
                return Conflict("Employee code already exists.");
            }

            if (await _context.Employees.AnyAsync(
                e => e.Email == dto.Email))
            {
                return Conflict("Email already exists.");
            }

            var employee = new Employee
            {
                EmployeeCode = dto.EmployeeCode,
                FullName = dto.FullName,
                Email = dto.Email,
                Location = dto.Location,
                MobileNumber = dto.MobileNumber,
                TrainingName = dto.TrainingName,
                TrainingCompletedOn = dto.TrainingCompletedOn
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var response = new EmployeeResponseDto
            {
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Email = employee.Email,
                Location = employee.Location,
                MobileNumber = employee.MobileNumber,
                TrainingName = employee.TrainingName,
                TrainingCompletedOn = employee.TrainingCompletedOn
            };

            return CreatedAtAction(
                nameof(GetEmployee),
                new { employeeCode = employee.EmployeeCode },
                response
            );
        }

        // PUT: api/employees/85010193
        [HttpPut("{employeeCode}")]
        public async Task<IActionResult> UpdateEmployee(
            string employeeCode,
            UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees.FindAsync(employeeCode);

            if (employee == null)
                return NotFound();

            if (await _context.Employees.AnyAsync(
                e => e.Email == dto.Email &&
                     e.EmployeeCode != employeeCode))
            {
                return Conflict("Email already exists.");
            }

            employee.FullName = dto.FullName;
            employee.Email = dto.Email;
            employee.Location = dto.Location;
            employee.MobileNumber = dto.MobileNumber;
            employee.TrainingName = dto.TrainingName;
            employee.TrainingCompletedOn = dto.TrainingCompletedOn;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/employees/85010193
        [HttpDelete("{employeeCode}")]
        public async Task<IActionResult> DeleteEmployee(string employeeCode)
        {
            var employee = await _context.Employees.FindAsync(employeeCode);

            if (employee == null)
                return NotFound();

            bool hasAllocations = await _context.ProjectAllocations
                .AnyAsync(a => a.EmployeeCode == employeeCode);

            if (hasAllocations)
                return Conflict(
                    "Cannot delete an employee with allocation history.");

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}