using System.ComponentModel.DataAnnotations;

namespace EmployeeProjectAllocation.DTOs.Skills
{
    public class AssignSkillsDto
    {
        [Required]
        public List<int> SkillIds { get; set; } = new();
    }
}