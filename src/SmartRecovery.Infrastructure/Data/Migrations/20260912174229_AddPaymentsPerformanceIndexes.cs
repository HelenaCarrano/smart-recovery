using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRecovery.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentsPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_DeclineReason",
                table: "Payments",
                column: "DeclineReason");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_DeclineReason",
                table: "Payments");
        }
    }
}
