using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeProjectAllocation.Migrations
{
    /// <inheritdoc />
    public partial class RestrictAllocationHistoryDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAllocations_Employees_EmployeeCode",
                table: "ProjectAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAllocations_Projects_ProjectId",
                table: "ProjectAllocations");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAllocations_Employees_EmployeeCode",
                table: "ProjectAllocations",
                column: "EmployeeCode",
                principalTable: "Employees",
                principalColumn: "EmployeeCode",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAllocations_Projects_ProjectId",
                table: "ProjectAllocations",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAllocations_Employees_EmployeeCode",
                table: "ProjectAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAllocations_Projects_ProjectId",
                table: "ProjectAllocations");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAllocations_Employees_EmployeeCode",
                table: "ProjectAllocations",
                column: "EmployeeCode",
                principalTable: "Employees",
                principalColumn: "EmployeeCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAllocations_Projects_ProjectId",
                table: "ProjectAllocations",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
