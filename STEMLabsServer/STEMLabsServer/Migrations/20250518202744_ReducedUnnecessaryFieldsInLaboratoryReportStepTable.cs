using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STEMLabsServer.Migrations
{
    /// <inheritdoc />
    public partial class ReducedUnnecessaryFieldsInLaboratoryReportStepTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLaboratoryReportSteps_LaboratorySessions_LaboratoryS~",
                table: "StudentLaboratoryReportSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLaboratoryReportSteps_Users_StudentId",
                table: "StudentLaboratoryReportSteps");

            migrationBuilder.DropIndex(
                name: "IX_StudentLaboratoryReportSteps_LaboratorySessionId",
                table: "StudentLaboratoryReportSteps");

            migrationBuilder.DropColumn(
                name: "LaboratorySessionId",
                table: "StudentLaboratoryReportSteps");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "StudentLaboratoryReportSteps",
                newName: "StudentLaboratoryReportId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLaboratoryReportSteps_StudentId",
                table: "StudentLaboratoryReportSteps",
                newName: "IX_StudentLaboratoryReportSteps_StudentLaboratoryReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLaboratoryReportSteps_StudentLaboratoryReports_Stude~",
                table: "StudentLaboratoryReportSteps",
                column: "StudentLaboratoryReportId",
                principalTable: "StudentLaboratoryReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLaboratoryReportSteps_StudentLaboratoryReports_Stude~",
                table: "StudentLaboratoryReportSteps");

            migrationBuilder.RenameColumn(
                name: "StudentLaboratoryReportId",
                table: "StudentLaboratoryReportSteps",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLaboratoryReportSteps_StudentLaboratoryReportId",
                table: "StudentLaboratoryReportSteps",
                newName: "IX_StudentLaboratoryReportSteps_StudentId");

            migrationBuilder.AddColumn<int>(
                name: "LaboratorySessionId",
                table: "StudentLaboratoryReportSteps",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentLaboratoryReportSteps_LaboratorySessionId",
                table: "StudentLaboratoryReportSteps",
                column: "LaboratorySessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLaboratoryReportSteps_LaboratorySessions_LaboratoryS~",
                table: "StudentLaboratoryReportSteps",
                column: "LaboratorySessionId",
                principalTable: "LaboratorySessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLaboratoryReportSteps_Users_StudentId",
                table: "StudentLaboratoryReportSteps",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
