
using EmployeeProjectAllocation.DTOs.Employees;

namespace EmployeeProjectAllocation.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeResponseDto>> GetAllEmployeesAsync();

        Task<EmployeeResponseDto?> GetEmployeeByCodeAsync(string employeeCode);

        Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto dto);

        Task<bool> UpdateEmployeeAsync(
            string employeeCode,
            UpdateEmployeeDto dto);

        Task<bool> DeleteEmployeeAsync(string employeeCode);
    }
}
