using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "MaintenanceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_VendorId",
                table: "MaintenanceRequests",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_Vendors_VendorId",
                table: "MaintenanceRequests",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_Vendors_VendorId",
                table: "MaintenanceRequests");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRequests_VendorId",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "MaintenanceRequests");
        }
    }
}
