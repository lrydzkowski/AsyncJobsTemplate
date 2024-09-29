using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsyncJobsTemplate.Infrastructure.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    RecId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 200, nullable: false),
                    JobCategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InputData = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true),
                    InputFileReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OutputData = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true),
                    OutputFileReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Errors = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.RecId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Job");
        }
    }
}
