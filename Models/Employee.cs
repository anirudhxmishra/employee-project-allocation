namespace EmployeeProjectAllocation.Models
{
    public class Employee
    {
        public string EmployeeCode { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string TrainingName { get; set; } = string.Empty;

        public DateTime? TrainingCompletedOn { get; set; }
        public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
    = new List<EmployeeSkill>();
    }
}