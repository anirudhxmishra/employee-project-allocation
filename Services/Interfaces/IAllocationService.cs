
using EmployeeProjectAllocation.DTOs.Allocations;

namespace EmployeeProjectAllocation.Services.Interfaces
{
    public interface IAllocationService
    {
        Task<List<AllocationResponseDto>> GetAllAllocationsAsync();

        Task<AllocationResponseDto?> GetAllocationByIdAsync(int id);

        Task<AllocationResponseDto> CreateAllocationAsync(
            CreateAllocationDto dto);

        Task<List<AllocationResponseDto>> GetEmployeeAllocationsAsync(
            string employeeCode);

        Task<bool> ReleaseAllocationAsync(
            int id,
            ReleaseAllocationDto dto);
    }
}
