using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRecovery.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecoveryScoreBreakdown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseScore",
                table: "RecoveryAnalyses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HistoryAdjustment",
                table: "RecoveryAnalyses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecentAttemptsAdjustment",
                table: "RecoveryAnalyses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecentDeclinesAdjustment",
                table: "RecoveryAnalyses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecoveryTrackRecordBonus",
                table: "RecoveryAnalyses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseScore",
                table: "RecoveryAnalyses");

            migrationBuilder.DropColumn(
                name: "HistoryAdjustment",
                table: "RecoveryAnalyses");

            migrationBuilder.DropColumn(
                name: "RecentAttemptsAdjustment",
                table: "RecoveryAnalyses");

            migrationBuilder.DropColumn(
                name: "RecentDeclinesAdjustment",
                table: "RecoveryAnalyses");

            migrationBuilder.DropColumn(
                name: "RecoveryTrackRecordBonus",
                table: "RecoveryAnalyses");
        }
    }
}
