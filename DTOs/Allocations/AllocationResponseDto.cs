namespace EmployeeProjectAllocation.DTOs.Allocations
{
    public class AllocationResponseDto
    {
        public int AllocationId { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime AllocationDate { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public bool IsActive { get; set; }

        public string AllocatedBy { get; set; } = string.Empty;
    }
}