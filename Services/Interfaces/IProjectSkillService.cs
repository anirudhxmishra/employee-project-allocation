
using EmployeeProjectAllocation.DTOs.ProjectSkills;

namespace EmployeeProjectAllocation.Services.Interfaces
{
    public interface IProjectSkillService
    {
        Task<List<ProjectSkillResponseDto>> GetProjectSkillsAsync(int projectId);

        Task<List<ProjectSkillResponseDto>> AssignProjectSkillsAsync(
            int projectId,
            AssignProjectSkillsDto dto);
    }
}
