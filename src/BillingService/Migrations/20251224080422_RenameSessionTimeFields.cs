using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Migrations
{
    /// <inheritdoc />
    public partial class RenameSessionTimeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Payments",
                newName: "SessionStarted");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Payments",
                newName: "SessionEnded");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SessionStarted",
                table: "Payments",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "SessionEnded",
                table: "Payments",
                newName: "EndTime");
        }
    }
}
