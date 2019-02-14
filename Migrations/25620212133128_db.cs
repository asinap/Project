using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace test2.Migrations
{
    public partial class db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id_account = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    Point = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id_account);
                });

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    Id_content = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlainText = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.Id_content);
                });

            migrationBuilder.CreateTable(
                name: "LockerMetadatas",
                columns: table => new
                {
                    Mac_address = table.Column<string>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockerMetadatas", x => x.Mac_address);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id_notification = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateandTime = table.Column<DateTime>(nullable: false),
                    IsShow = table.Column<bool>(nullable: false),
                    Mac_address = table.Column<string>(nullable: true),
                    Id_vacant = table.Column<int>(nullable: false),
                    Id_content = table.Column<int>(nullable: false),
                    Id_account = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id_notification);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id_reserve = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    StartDay = table.Column<DateTime>(nullable: false),
                    EndDay = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Optional = table.Column<int>(nullable: false),
                    Size = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Id_account = table.Column<string>(nullable: true),
                    Id_vacancy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id_reserve);
                });

            migrationBuilder.CreateTable(
                name: "Vacancies",
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
                    table.PrimaryKey("PK_Vacancies", x => x.Id_vacancy);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.DropTable(
                name: "LockerMetadatas");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Vacancies");
        }
    }
}
