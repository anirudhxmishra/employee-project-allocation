
using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.ProjectCandidates
{
    public class UpdateCandidateStatusDto
    {
        [Required]
        [RegularExpression(
            "^(Shortlisted|InterviewScheduled|Selected|Rejected|OnHold)$",
            ErrorMessage = "Invalid status. Allowed values: Shortlisted, InterviewScheduled, Selected, Rejected, OnHold.")]
        public string Status { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Role { get; set; }

        public DateTime? InterviewDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
