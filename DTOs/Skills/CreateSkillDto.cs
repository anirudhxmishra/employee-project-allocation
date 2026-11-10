using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Skills
{
    public class CreateSkillDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string SkillName { get; set; } = string.Empty;
    }
}