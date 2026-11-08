using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Allocations
{
    public class CreateAllocationDto
    {
        [Required]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Role { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string AllocatedBy { get; set; } = string.Empty;
    }
}