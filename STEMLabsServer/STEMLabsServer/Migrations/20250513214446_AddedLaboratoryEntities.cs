using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace STEMLabsServer.Migrations
{
    /// <inheritdoc />
    public partial class AddedLaboratoryEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Laboratories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SceneId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratories", x => x.Id);
                    table.UniqueConstraint("AK_Laboratories_SceneId", x => x.SceneId);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryChecklistSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LaboratoryId = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    StepNumber = table.Column<int>(type: "integer", nullable: false),
                    Statement = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryChecklistSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryChecklistSteps_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.UniqueConstraint("AK_LaboratoryChecklistSteps_LaboratoryId_Version_StepNumber",
                        x => new { x.LaboratoryId, x.StepNumber, x.Version });
                });

            migrationBuilder.CreateTable(
                name: "LaboratorySessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LaboratoryId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InviteCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratorySessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratorySessions_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboratorySessions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentLaboratoryCompletedSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LaboratorySessionId = table.Column<int>(type: "integer", nullable: false),
                    LaboratoryChecklistStepId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLaboratoryCompletedSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentLaboratoryCompletedSteps_LaboratoryChecklistSteps_La~",
                        column: x => x.LaboratoryChecklistStepId,
                        principalTable: "LaboratoryChecklistSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentLaboratoryCompletedSteps_LaboratorySessions_Laborato~",
                        column: x => x.LaboratorySessionId,
                        principalTable: "LaboratorySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentLaboratoryCompletedSteps_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentLaboratoryReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LaboratorySessionId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    ObservationsImageLink = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLaboratoryReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentLaboratoryReports_LaboratorySessions_LaboratorySessi~",
                        column: x => x.LaboratorySessionId,
                        principalTable: "LaboratorySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentLaboratoryReports_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryChecklistSteps_LaboratoryId",
                table: "LaboratoryChecklistSteps",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratorySessions_CreatedById",
                table: "LaboratorySessions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratorySessions_LaboratoryId",
                table: "LaboratorySessions",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLaboratoryCompletedSteps_LaboratoryChecklistStepId",
                table: "StudentLaboratoryCompletedSteps",
                column: "LaboratoryChecklistStepId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLaboratoryCompletedSteps_LaboratorySessionId",
                table: "StudentLaboratoryCompletedSteps",
                column: "LaboratorySessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLaboratoryCompletedSteps_StudentId",
                table: "StudentLaboratoryCompletedSteps",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLaboratoryReports_LaboratorySessionId",
                table: "StudentLaboratoryReports",
                column: "LaboratorySessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLaboratoryReports_StudentId",
                table: "StudentLaboratoryReports",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentLaboratoryCompletedSteps");

            migrationBuilder.DropTable(
                name: "StudentLaboratoryReports");

            migrationBuilder.DropTable(
                name: "LaboratoryChecklistSteps");

            migrationBuilder.DropTable(
                name: "LaboratorySessions");

            migrationBuilder.DropTable(
                name: "Laboratories");
        }
    }
}
