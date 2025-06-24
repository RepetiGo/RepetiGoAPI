using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepetiGo.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEasyBonusAndFailedInterval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EasyBonus",
                table: "Settings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FailedInterval",
                table: "Cards",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EasyBonus",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FailedInterval",
                table: "Cards");
        }
    }
}
