using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeProjectAllocation.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectAllocations_EmployeeCode",
                table: "ProjectAllocations");

            migrationBuilder.AlterColumn<string>(
                name: "SkillName",
                table: "Skills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SkillName",
                table: "Skills",
                column: "SkillName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_EmployeeCode",
                table: "ProjectAllocations",
                column: "EmployeeCode",
                unique: true,
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Skills_SkillName",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAllocations_EmployeeCode",
                table: "ProjectAllocations");

            migrationBuilder.AlterColumn<string>(
                name: "SkillName",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_EmployeeCode",
                table: "ProjectAllocations",
                column: "EmployeeCode");
        }
    }
}
