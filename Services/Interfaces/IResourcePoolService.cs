
using EmployeeProjectAllocation.DTOs.ResourcePool;

namespace EmployeeProjectAllocation.Services.Interfaces
{
    public interface IResourcePoolService
    {
        Task<List<ResourcePoolEmployeeDto>> GetAvailableEmployeesAsync(
            int? projectId = null);
    }
}
