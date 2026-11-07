namespace EmployeeProjectAllocation.Models
{
    public class Skill
    {
        public int SkillId { get; set; }

        public string SkillName { get; set; } = string.Empty;
        public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
    = new List<EmployeeSkill>();
    }
}