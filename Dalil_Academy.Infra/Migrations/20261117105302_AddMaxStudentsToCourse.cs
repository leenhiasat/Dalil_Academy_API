using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dalil_Academy.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxStudentsToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxStudents",
                table: "Courses",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxStudents",
                table: "Courses");
        }
    }
}
