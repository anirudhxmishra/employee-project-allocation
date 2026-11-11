
using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.ProjectCandidates
{
    public class CreateProjectCandidateDto
    {
        [Required]
        [StringLength(8, MinimumLength = 1)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ProjectId { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
