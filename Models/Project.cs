namespace EmployeeProjectAllocation.Models
{
    public class Project
    {
        public int ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;

        public string Technology { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = "Active";
    }
}