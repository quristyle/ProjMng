using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace skgRestApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ghub");

            migrationBuilder.RenameTable(
                name: "WeatherInfos",
                newName: "WeatherInfos",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "UserProfiles",
                newName: "UserProfiles",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "SfmReports",
                newName: "SfmReports",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "SafetyViolations",
                newName: "SafetyViolations",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "SafetyStickers",
                newName: "SafetyStickers",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "SafetyStandards",
                newName: "SafetyStandards",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "ReferenceStandards",
                newName: "ReferenceStandards",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "MealMenus",
                newName: "MealMenus",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "MealMenuItems",
                newName: "MealMenuItems",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "MealFeedbacks",
                newName: "MealFeedbacks",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "ConnectHubItems",
                newName: "ConnectHubItems",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "BirthdayEntries",
                newName: "BirthdayEntries",
                newSchema: "ghub");

            migrationBuilder.RenameTable(
                name: "AppSettings",
                newName: "AppSettings",
                newSchema: "ghub");

            migrationBuilder.CreateTable(
                name: "LoginHistories",
                schema: "ghub",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    FailureReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "ghub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Revoked = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginHistories",
                schema: "ghub");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "ghub");

            migrationBuilder.RenameTable(
                name: "WeatherInfos",
                schema: "ghub",
                newName: "WeatherInfos");

            migrationBuilder.RenameTable(
                name: "UserProfiles",
                schema: "ghub",
                newName: "UserProfiles");

            migrationBuilder.RenameTable(
                name: "SfmReports",
                schema: "ghub",
                newName: "SfmReports");

            migrationBuilder.RenameTable(
                name: "SafetyViolations",
                schema: "ghub",
                newName: "SafetyViolations");

            migrationBuilder.RenameTable(
                name: "SafetyStickers",
                schema: "ghub",
                newName: "SafetyStickers");

            migrationBuilder.RenameTable(
                name: "SafetyStandards",
                schema: "ghub",
                newName: "SafetyStandards");

            migrationBuilder.RenameTable(
                name: "ReferenceStandards",
                schema: "ghub",
                newName: "ReferenceStandards");

            migrationBuilder.RenameTable(
                name: "MealMenus",
                schema: "ghub",
                newName: "MealMenus");

            migrationBuilder.RenameTable(
                name: "MealMenuItems",
                schema: "ghub",
                newName: "MealMenuItems");

            migrationBuilder.RenameTable(
                name: "MealFeedbacks",
                schema: "ghub",
                newName: "MealFeedbacks");

            migrationBuilder.RenameTable(
                name: "ConnectHubItems",
                schema: "ghub",
                newName: "ConnectHubItems");

            migrationBuilder.RenameTable(
                name: "BirthdayEntries",
                schema: "ghub",
                newName: "BirthdayEntries");

            migrationBuilder.RenameTable(
                name: "AppSettings",
                schema: "ghub",
                newName: "AppSettings");
        }
    }
}
