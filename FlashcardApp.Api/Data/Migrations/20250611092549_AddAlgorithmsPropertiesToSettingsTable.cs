using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardApp.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAlgorithmsPropertiesToSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Settings_UserId_Key",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Settings");

            migrationBuilder.AddColumn<int>(
                name: "FirstSuccessInterval",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LapseIntervalDays",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxReviewsPerDay",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NewCardsPerDay",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecondSuccessInterval",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "StartingEasinessFactor",
                table: "Settings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextReview",
                table: "Cards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserId",
                table: "Settings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Settings_UserId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FirstSuccessInterval",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "LapseIntervalDays",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "MaxReviewsPerDay",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "NewCardsPerDay",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "SecondSuccessInterval",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "StartingEasinessFactor",
                table: "Settings");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Settings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Value",
                table: "Settings",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextReview",
                table: "Cards",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserId_Key",
                table: "Settings",
                columns: new[] { "UserId", "Key" },
                unique: true);
        }
    }
}
