
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

        public async Task<List<EmployeeResponseDto>> GetAllEmployeesAsync()
        {
            return await _context.Employees
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

        public async Task<EmployeeResponseDto?> GetEmployeeByCodeAsync(
            string employeeCode)
        {
            return await _context.Employees
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

        public async Task<EmployeeResponseDto> CreateEmployeeAsync(
            CreateEmployeeDto dto)
        {
            var employeeCode = dto.EmployeeCode.Trim();
            var email = dto.Email.Trim();

            if (await _context.Employees.AnyAsync(
                e => e.EmployeeCode == employeeCode))
            {
                throw new InvalidOperationException(
                    "Employee code already exists.");
            }

            if (await _context.Employees.AnyAsync(
                e => e.Email == email))
            {
                throw new InvalidOperationException(
                    "Email already exists.");
            }

            var employee = new Employee
            {
                EmployeeCode = employeeCode,
                FullName = dto.FullName.Trim(),
                Email = email,
                Location = dto.Location,
                MobileNumber = dto.MobileNumber,
                TrainingName = dto.TrainingName,
                TrainingCompletedOn = dto.TrainingCompletedOn
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

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

        public async Task<bool> UpdateEmployeeAsync(
            string employeeCode,
            UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees
                .FindAsync(employeeCode);

            if (employee == null)
                return false;

            var email = dto.Email.Trim();

            var emailExists = await _context.Employees.AnyAsync(
                e => e.Email == email &&
                     e.EmployeeCode != employeeCode);

            if (emailExists)
                throw new InvalidOperationException(
                    "Email already exists.");

            employee.FullName = dto.FullName.Trim();
            employee.Email = email;
            employee.Location = dto.Location;
            employee.MobileNumber = dto.MobileNumber;
            employee.TrainingName = dto.TrainingName;
            employee.TrainingCompletedOn = dto.TrainingCompletedOn;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(string employeeCode)
        {
            var employee = await _context.Employees
                .FindAsync(employeeCode);

            if (employee == null)
                return false;

            var hasAllocations = await _context.ProjectAllocations
                .AnyAsync(a => a.EmployeeCode == employeeCode);

            if (hasAllocations)
                throw new InvalidOperationException(
                    "Cannot delete an employee with allocation history.");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
