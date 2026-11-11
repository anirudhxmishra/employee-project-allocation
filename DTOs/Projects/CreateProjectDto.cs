using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Projects
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string ProjectCode { get; set; } = string.Empty;

        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ClientName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Technology { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int RequiredEmployees { get; set; } = 1;

        [StringLength(100)]
        public string? Location { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}