using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace test2.Migrations
{
    public partial class db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    Id_account = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    Point = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.Id_account);
                });

            migrationBuilder.CreateTable(
                name: "contents",
                columns: table => new
                {
                    Id_content = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlainText = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contents", x => x.Id_content);
                });

            migrationBuilder.CreateTable(
                name: "lockerMetadatas",
                columns: table => new
                {
                    Mac_address = table.Column<string>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lockerMetadatas", x => x.Mac_address);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id_notification = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    IsShow = table.Column<bool>(nullable: false),
                    Id_reserve = table.Column<int>(nullable: false),
                    Read = table.Column<bool>(nullable: false),
                    Id_content = table.Column<int>(nullable: false),
                    Id_account = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id_notification);
                });

            migrationBuilder.CreateTable(
                name: "notiTokens",
                columns: table => new
                {
                    Id_account = table.Column<string>(nullable: false),
                    ExpoToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notiTokens", x => x.Id_account);
                });

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    Id_reserve = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    StartDay = table.Column<DateTime>(nullable: false),
                    EndDay = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Size = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Id_account = table.Column<string>(nullable: true),
                    Id_vacancy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservations", x => x.Id_reserve);
                });

            migrationBuilder.CreateTable(
                name: "vacancies",
                columns: table => new
                {
                    Id_vacancy = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    No_vacancy = table.Column<string>(nullable: true),
                    Size = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Mac_address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacancies", x => x.Id_vacancy);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "contents");

            migrationBuilder.DropTable(
                name: "lockerMetadatas");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "notiTokens");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "vacancies");
        }
    }
}
