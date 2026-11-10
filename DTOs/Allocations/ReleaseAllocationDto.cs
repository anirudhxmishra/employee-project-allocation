using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Allocations
{
    public class ReleaseAllocationDto
    {
        [Required]
        [Range(typeof(DateTime), "2000-01-01", "2100-12-31",
            ErrorMessage = "Please provide a valid release date.")]
        public DateTime ReleaseDate { get; set; }
    }
}