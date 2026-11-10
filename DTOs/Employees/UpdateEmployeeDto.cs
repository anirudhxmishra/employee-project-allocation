using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Employees
{
    public class UpdateEmployeeDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Mobile number must contain exactly 10 digits.")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string TrainingName { get; set; } = string.Empty;

        public DateTime? TrainingCompletedOn { get; set; }
    }
}