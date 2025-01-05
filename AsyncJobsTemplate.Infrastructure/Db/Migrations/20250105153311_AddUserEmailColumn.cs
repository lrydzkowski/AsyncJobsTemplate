using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsyncJobsTemplate.Infrastructure.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEmailColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Job",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Job");
        }
    }
}
