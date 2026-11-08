
namespace EmployeeProjectAllocation.DTOs.Dashboard
{
    public class DashboardResponseDto
    {
        public int TotalEmployees { get; set; }

        public int AllocatedEmployees { get; set; }

        public int AvailableEmployees { get; set; }

        public int TotalProjects { get; set; }

        public int ActiveProjects { get; set; }

        public int TotalAllocations { get; set; }

        public int ActiveAllocations { get; set; }
    }
}
