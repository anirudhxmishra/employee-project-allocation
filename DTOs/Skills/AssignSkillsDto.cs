using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Skills
{
    public class AssignSkillsDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one skill must be selected.")]
        public List<int> SkillIds { get; set; } = new();
    }
}