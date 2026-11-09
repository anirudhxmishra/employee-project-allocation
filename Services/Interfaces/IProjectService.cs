
using EmployeeProjectAllocation.DTOs.Projects;

namespace EmployeeProjectAllocation.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectResponseDto>> GetAllProjectsAsync();

        Task<ProjectResponseDto?> GetProjectByIdAsync(int id);

        Task<ProjectResponseDto> CreateProjectAsync(
            CreateProjectDto dto);

        Task<bool> UpdateProjectAsync(
            int id,
            UpdateProjectDto dto);

        Task<bool> DeleteProjectAsync(int id);
    }
}
