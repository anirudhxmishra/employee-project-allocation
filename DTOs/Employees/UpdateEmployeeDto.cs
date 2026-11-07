namespace EmployeeProjectAllocation.DTOs.Employees
{
    public class UpdateEmployeeDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string TrainingName { get; set; } = string.Empty;
        public DateTime? TrainingCompletedOn { get; set; }
    }
}