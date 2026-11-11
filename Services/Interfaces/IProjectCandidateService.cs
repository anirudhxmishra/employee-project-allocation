
using EmployeeProjectAllocation.DTOs.ProjectCandidates;

namespace EmployeeProjectAllocation.Services.Interfaces
{
    public interface IProjectCandidateService
    {
        // Shortlist an employee
        Task<ProjectCandidateResponseDto> CreateCandidateAsync(
            CreateProjectCandidateDto dto);

        // Get all candidates for a project
        Task<List<ProjectCandidateResponseDto>> GetProjectCandidatesAsync(
            int projectId);

        // Update candidate status and handle automatic allocation
        Task<ProjectCandidateResponseDto> UpdateCandidateStatusAsync(
            int candidateId,
            UpdateCandidateStatusDto dto,
            string allocatedBy);
    }
}
