using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Allocations
{
    public class ReleaseAllocationDto
    {
        [Required]
        public DateTime ReleaseDate { get; set; }
    }
}