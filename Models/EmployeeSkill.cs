namespace EmployeeProjectAllocation.Models
{
    public class EmployeeSkill
    {
        public string EmployeeCode { get; set; } = string.Empty;

        public int SkillId { get; set; }

        public Employee Employee { get; set; } = null!;

        public Skill Skill { get; set; } = null!;
    }
}