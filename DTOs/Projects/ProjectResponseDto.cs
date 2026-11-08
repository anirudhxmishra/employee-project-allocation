namespace EmployeeProjectAllocation.DTOs.Projects
{
    public class ProjectResponseDto
    {
        public int ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;

        public string? Technology { get; set; }

        public string? Location { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}