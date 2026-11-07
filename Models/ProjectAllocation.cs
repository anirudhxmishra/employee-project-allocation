namespace EmployeeProjectAllocation.Models
{
    public class ProjectAllocation
    {
        public int AllocationId { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public string Role { get; set; } = string.Empty;

        public DateTime AllocationDate { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public bool IsActive { get; set; } = true;

        public string AllocatedBy { get; set; } = string.Empty;

        public Employee Employee { get; set; } = null!;

        public Project Project { get; set; } = null!;
    }
}