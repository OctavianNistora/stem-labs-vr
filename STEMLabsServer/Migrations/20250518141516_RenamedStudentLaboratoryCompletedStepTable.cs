using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STEMLabsServer.Migrations
{
    /// <inheritdoc />
    public partial class RenamedStudentLaboratoryCompletedStepTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLaboratoryCompletedSteps_LaboratoryChecklistSteps_La~",
                table: "StudentLaboratoryCompletedSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLaboratoryCompletedSteps_LaboratorySessions_Laborato~",
                table: "StudentLaboratoryCompletedSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLaboratoryCompletedSteps_Users_StudentId",
                table: "StudentLaboratoryCompletedSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentLaboratoryCompletedSteps",
                table: "StudentLaboratoryCompletedSteps");

            migrationBuilder.RenameTable(
                name: "StudentLaboratoryCompletedSteps",
                newName: "StudentLaboratoryReportSteps");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLaboratoryCompletedSteps_StudentId",
                table: "StudentLaboratoryReportSteps",
                newName: "IX_StudentLaboratoryReportSteps_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLaboratoryCompletedSteps_LaboratorySessionId",
                table: "StudentLaboratoryReportSteps",
                newName: "IX_StudentLaboratoryReportSteps_LaboratorySessionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLaboratoryCompletedSteps_LaboratoryChecklistStepId",
                table: "StudentLaboratoryReportSteps",
                newName: "IX_StudentLaboratoryReportSteps_LaboratoryChecklistStepId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentLaboratoryReportSteps",
                table: "StudentLaboratoryReportSteps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLaboratoryReportSteps_LaboratoryChecklistSteps_Labor~",
                table: "StudentLaboratoryReportSteps",
                column: "LaboratoryChecklistStepId",
                principalTable: "LaboratoryChecklistSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLaboratoryReportSteps_LaboratoryChecklistSteps_Labor~",
                table: "StudentLaboratoryReportSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLaboratoryReportSteps_LaboratorySessions_LaboratoryS~",
                table: "StudentLaboratoryReportSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLaboratoryReportSteps_Users_StudentId",
                table: "StudentLaboratoryReportSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentLaboratoryReportSteps",
                table: "StudentLaboratoryReportSteps");

            migrationBuilder.RenameTable(
                name: "StudentLaboratoryReportSteps",
                newName: "StudentLaboratoryCompletedSteps");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLaboratoryReportSteps_StudentId",
                table: "StudentLaboratoryCompletedSteps",
                newName: "IX_StudentLaboratoryCompletedSteps_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLaboratoryReportSteps_LaboratorySessionId",
                table: "StudentLaboratoryCompletedSteps",
                newName: "IX_StudentLaboratoryCompletedSteps_LaboratorySessionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentLaboratoryReportSteps_LaboratoryChecklistStepId",
                table: "StudentLaboratoryCompletedSteps",
                newName: "IX_StudentLaboratoryCompletedSteps_LaboratoryChecklistStepId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentLaboratoryCompletedSteps",
                table: "StudentLaboratoryCompletedSteps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLaboratoryCompletedSteps_LaboratoryChecklistSteps_La~",
                table: "StudentLaboratoryCompletedSteps",
                column: "LaboratoryChecklistStepId",
                principalTable: "LaboratoryChecklistSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLaboratoryCompletedSteps_LaboratorySessions_Laborato~",
                table: "StudentLaboratoryCompletedSteps",
                column: "LaboratorySessionId",
                principalTable: "LaboratorySessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLaboratoryCompletedSteps_Users_StudentId",
                table: "StudentLaboratoryCompletedSteps",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
