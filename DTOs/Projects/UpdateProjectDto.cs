using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Projects
{
    public class UpdateProjectDto
    {
        [Required]
        [MaxLength(150)]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ClientName { get; set; } = string.Empty;

        public string? Technology { get; set; }

        public string? Location { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "Active";
    }
}