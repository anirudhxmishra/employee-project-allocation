
namespace EmployeeProjectAllocation.DTOs.Hiring
{
    public class ProjectHiringSummaryDto
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public int RequiredEmployees { get; set; }

        public int ActiveAllocations { get; set; }

        public int RemainingVacancies { get; set; }

        public int ShortlistedCount { get; set; }

        public int InterviewScheduledCount { get; set; }

        public int SelectedCount { get; set; }

        public int RejectedCount { get; set; }

        public int OnHoldCount { get; set; }
    }
}
