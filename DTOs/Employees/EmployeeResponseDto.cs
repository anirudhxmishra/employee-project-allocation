
namespace EmployeeProjectAllocation.DTOs.Employees
{
    public class EmployeeResponseDto
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? MobileNumber { get; set; }
        public string? TrainingName { get; set; }
        public DateTime? TrainingCompletedOn { get; set; }
    }
}
