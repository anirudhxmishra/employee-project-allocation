using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Allocations
{
    public class CreateAllocationDto
    {
        [Required]
        [StringLength(8, MinimumLength = 1)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Range(1, int.MaxValue,
            ErrorMessage = "Please provide a valid Project ID.")]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Role { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string AllocatedBy { get; set; } = string.Empty;
    }
}