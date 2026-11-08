using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Projects
{
    public class CreateProjectDto
    {
        [Required]
        [MaxLength(20)]
        public string ProjectCode { get; set; } = string.Empty;

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
    }
}