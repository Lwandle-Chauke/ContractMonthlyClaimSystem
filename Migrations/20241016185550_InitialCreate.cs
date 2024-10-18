using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace PROGPart2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the Claims table
            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LecturerName = table.Column<string>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    HoursWorked = table.Column<int>(nullable: false),
                    HourlyRate = table.Column<decimal>(nullable: false),
                    University = table.Column<string>(nullable: false),
                    DocumentPath = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: false, defaultValue: "Pending"),
                    LecturerEmail = table.Column<string>(nullable: false),
                    Feedback = table.Column<string>(nullable: true),
                    ProgrammeCoordinatorName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                });

            // Alter the Password column in the Users table
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Additional changes for the Claims table (Adding Columns)
            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LecturerEmail",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LecturerName",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProgrammeCoordinatorName",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
    name: "HourlyRate",
    table: "Claims",
    type: "decimal(18, 2)", // Specify precision and scale here
    nullable: false,
    oldClrType: typeof(decimal),
    oldType: "decimal");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the Claims table when rolling back
            migrationBuilder.DropTable(
                name: "Claims");

            // Revert the changes to the Claims table
            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "LecturerEmail",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "LecturerName",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ProgrammeCoordinatorName",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Claims");

            // Revert the changes to the Users table
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
