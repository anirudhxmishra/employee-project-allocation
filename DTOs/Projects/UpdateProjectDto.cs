using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Projects
{
    public class UpdateProjectDto
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ClientName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Technology { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        [RegularExpression(
            "^(Active|Inactive|Completed)$",
            ErrorMessage = "Status must be Active, Inactive, or Completed."
        )]
        public string Status { get; set; } = "Active";
    }
}