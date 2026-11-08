using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Skills
{
    public class CreateSkillDto
    {
        [Required]
        [MaxLength(100)]
        public string SkillName { get; set; } = string.Empty;
    }
}