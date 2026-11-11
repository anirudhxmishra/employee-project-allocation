
namespace EmployeeProjectAllocation.DTOs.ProjectCandidates
{
    public class ProjectCandidateResponseDto
    {
        public int CandidateId { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime ShortlistedOn { get; set; }

        public DateTime? InterviewDate { get; set; }

        public string? Remarks { get; set; }
    }
}
