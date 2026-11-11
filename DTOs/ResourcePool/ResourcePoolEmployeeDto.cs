
namespace EmployeeProjectAllocation.DTOs.ResourcePool
{
    public class ResourcePoolEmployeeDto
    {
        public string EmployeeCode { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Location { get; set; }

        public string? MobileNumber { get; set; }

        public string? TrainingName { get; set; }

        public DateTime? TrainingCompletedOn { get; set; }

        public List<string> Skills { get; set; } = new();

        public List<ResourcePoolCandidateProjectDto> ShortlistedProjects { get; set; } = new();
    }

    public class ResourcePoolCandidateProjectDto
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
