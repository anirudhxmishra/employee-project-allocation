namespace EmployeeProjectAllocation.Models
{
    public class ProjectCandidate
    {
        public int CandidateId { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public string Status { get; set; } = "Shortlisted";

        public DateTime ShortlistedOn { get; set; }

        public DateTime? InterviewDate { get; set; }

        public string? Remarks { get; set; }

        public Employee Employee { get; set; } = null!;

        public Project Project { get; set; } = null!;
    }
}