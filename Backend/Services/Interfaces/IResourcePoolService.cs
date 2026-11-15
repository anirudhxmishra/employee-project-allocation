using EmployeeProjectAllocation.Backend.DTOs.ResourcePool;

namespace EmployeeProjectAllocation.Backend.Services.Interfaces
{
    public interface IResourcePoolService
    {
        Task<List<ResourcePoolEmployeeDto>> GetAvailableEmployeesAsync(
            int? projectId = null);
    }
}
