using EmployeeProjectAllocation.Backend.DTOs.Skills;

namespace EmployeeProjectAllocation.Backend.Services.Interfaces
{
    public interface ISkillService
    {
        Task<List<SkillResponseDto>> GetAllSkillsAsync();

        Task<SkillResponseDto> CreateSkillAsync(CreateSkillDto dto);

        Task<bool> AssignSkillsAsync(
            string employeeCode,
            AssignSkillsDto dto);

        Task<List<SkillResponseDto>> GetEmployeeSkillsAsync(
            string employeeCode);
    }
}
