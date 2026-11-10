
using EmployeeProjectAllocation.Data;
using EmployeeProjectAllocation.DTOs.Employees;
using EmployeeProjectAllocation.Models;
using EmployeeProjectAllocation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectAllocation.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        // GET: All employees
        public async Task<List<EmployeeResponseDto>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .OrderBy(e => e.EmployeeCode)
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
        }

        // GET: Employee by code
        public async Task<EmployeeResponseDto?> GetEmployeeByCodeAsync(
            string employeeCode)
        {
            employeeCode = employeeCode.Trim();

            return await _context.Employees
                .AsNoTracking()
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
        }

        // POST: Create employee
        public async Task<EmployeeResponseDto> CreateEmployeeAsync(
            CreateEmployeeDto dto)
        {
            var employeeCode = dto.EmployeeCode.Trim();
            var email = dto.Email.Trim();

            var employeeCodeExists = await _context.Employees
                .AnyAsync(e =>
                    e.EmployeeCode.ToLower() == employeeCode.ToLower());

            if (employeeCodeExists)
            {
                throw new InvalidOperationException(
                    "Employee code already exists.");
            }

            var emailExists = await _context.Employees
                .AnyAsync(e =>
                    e.Email.ToLower() == email.ToLower());

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "Email already exists.");
            }

            var employee = new Employee
            {
                EmployeeCode = employeeCode,
                FullName = dto.FullName.Trim(),
                Email = email,
                Location = NormalizeOptionalString(dto.Location),
                MobileNumber = NormalizeOptionalString(dto.MobileNumber),
                TrainingName = NormalizeOptionalString(dto.TrainingName),
                TrainingCompletedOn = dto.TrainingCompletedOn
            };

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return MapToResponseDto(employee);
        }

        // PUT: Update employee
        public async Task<bool> UpdateEmployeeAsync(
            string employeeCode,
            UpdateEmployeeDto dto)
        {
            employeeCode = employeeCode.Trim();

            var employee = await _context.Employees
                .FindAsync(employeeCode);

            if (employee == null)
                return false;

            var email = dto.Email.Trim();

            var emailExists = await _context.Employees
                .AnyAsync(e =>
                    e.Email.ToLower() == email.ToLower() &&
                    e.EmployeeCode != employeeCode);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "Email already exists.");
            }

            employee.FullName = dto.FullName.Trim();
            employee.Email = email;
            employee.Location = NormalizeOptionalString(dto.Location);
            employee.MobileNumber = NormalizeOptionalString(dto.MobileNumber);
            employee.TrainingName = NormalizeOptionalString(dto.TrainingName);
            employee.TrainingCompletedOn = dto.TrainingCompletedOn;

            await _context.SaveChangesAsync();

            return true;
        }

        // DELETE: Delete employee
        public async Task<bool> DeleteEmployeeAsync(string employeeCode)
        {
            employeeCode = employeeCode.Trim();

            var employee = await _context.Employees
                .FindAsync(employeeCode);

            if (employee == null)
                return false;

            var hasAllocations = await _context.ProjectAllocations
                .AnyAsync(a => a.EmployeeCode == employeeCode);

            if (hasAllocations)
            {
                throw new InvalidOperationException(
                    "Cannot delete an employee with allocation history.");
            }

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return true;
        }

        // Normalize optional strings
        private static string? NormalizeOptionalString(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        // Map entity to response DTO
        private static EmployeeResponseDto MapToResponseDto(Employee employee)
        {
            return new EmployeeResponseDto
            {
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Email = employee.Email,
                Location = employee.Location,
                MobileNumber = employee.MobileNumber,
                TrainingName = employee.TrainingName,
                TrainingCompletedOn = employee.TrainingCompletedOn
            };
        }
    }
}
