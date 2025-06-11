using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardApp.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LearningSteps",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LearningStep",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearningSteps",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "LearningStep",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cards");
        }
    }
}
