using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace skgRestApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Section = table.Column<string>(type: "text", nullable: true),
                    DataType = table.Column<string>(type: "text", nullable: true),
                    IsSensitive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BirthdayEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsLunar = table.Column<bool>(type: "boolean", nullable: false),
                    IsCelebrated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConnectHubItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    IsExternal = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectHubItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MealMenuId = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    Likes = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealFeedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealMenus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceStandards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NumericValue = table.Column<decimal>(type: "numeric", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceStandards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SafetyStandards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Details = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyStandards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SafetyStickers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StickerCode = table.Column<string>(type: "text", nullable: false),
                    RecipientId = table.Column<string>(type: "text", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    IssuedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyStickers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SafetyViolations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ViolatorId = table.Column<string>(type: "text", nullable: false),
                    ObservedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: true),
                    ViolationType = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionTaken = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyViolations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SfmReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReporterId = table.Column<string>(type: "text", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MeasurementScore = table.Column<decimal>(type: "numeric", nullable: false),
                    Feedback = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    IsAcknowledged = table.Column<bool>(type: "boolean", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfmReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Department = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<string>(type: "text", nullable: true),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    PreferencesJson = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Location = table.Column<string>(type: "text", nullable: false),
                    ObservationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TemperatureC = table.Column<double>(type: "double precision", nullable: false),
                    Condition = table.Column<string>(type: "text", nullable: false),
                    Humidity = table.Column<int>(type: "integer", nullable: true),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: true),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealMenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Calories = table.Column<int>(type: "integer", nullable: true),
                    AllergyInfo = table.Column<string>(type: "text", nullable: true),
                    MealMenuId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionService = table.Column<string>(type: "text", nullable: true),
                    MenuContext = table.Column<string>(type: "text", nullable: true),
                    RemoteAddr = table.Column<string>(type: "text", nullable: true),
                    RemoteMchineInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealMenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealMenuItems_MealMenus_MealMenuId",
                        column: x => x.MealMenuId,
                        principalTable: "MealMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Key",
                table: "AppSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealMenuItems_MealMenuId",
                table: "MealMenuItems",
                column: "MealMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceStandards_Key",
                table: "ReferenceStandards",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SafetyStandards_Code",
                table: "SafetyStandards",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "BirthdayEntries");

            migrationBuilder.DropTable(
                name: "ConnectHubItems");

            migrationBuilder.DropTable(
                name: "MealFeedbacks");

            migrationBuilder.DropTable(
                name: "MealMenuItems");

            migrationBuilder.DropTable(
                name: "ReferenceStandards");

            migrationBuilder.DropTable(
                name: "SafetyStandards");

            migrationBuilder.DropTable(
                name: "SafetyStickers");

            migrationBuilder.DropTable(
                name: "SafetyViolations");

            migrationBuilder.DropTable(
                name: "SfmReports");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "WeatherInfos");

            migrationBuilder.DropTable(
                name: "MealMenus");
        }
    }
}
