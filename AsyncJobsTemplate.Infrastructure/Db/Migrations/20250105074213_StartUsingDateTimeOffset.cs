using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsyncJobsTemplate.Infrastructure.Db.Migrations
{
    /// <inheritdoc />
    public partial class StartUsingDateTimeOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAtUtc",
                table: "Job");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Job",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedAt",
                table: "Job",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Job");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Job",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAtUtc",
                table: "Job",
                type: "datetime2",
                nullable: true);
        }
    }
}
