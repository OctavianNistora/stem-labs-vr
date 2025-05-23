using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STEMLabsServer.Migrations
{
    /// <inheritdoc />
    public partial class AddedCheckListCountToLaboratoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CheckListStepCount",
                table: "Laboratories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckListStepCount",
                table: "Laboratories");
        }
    }
}
