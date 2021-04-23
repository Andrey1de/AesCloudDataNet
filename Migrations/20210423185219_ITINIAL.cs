using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace AesCloudDataNet.Migrations
{
    public partial class ITINIAL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RateToUsd",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(4)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Rate = table.Column<double>(type: "double", nullable: false),
                    Bid = table.Column<double>(type: "double", nullable: false),
                    Ask = table.Column<double>(type: "double", nullable: false),
                    Stored = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastRefreshed = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateToUsd", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "UserActions",
                columns: table => new
                {
                    UserActionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    NextActionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    User = table.Column<string>(type: "text", nullable: true),
                    PriodSec = table.Column<int>(type: "int", nullable: true),
                    Blob = table.Column<byte[]>(type: "varbinary(4000)", nullable: true),
                    Json = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActions", x => x.UserActionId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", nullable: true),
                    Password = table.Column<byte[]>(type: "varbinary(512)", nullable: true),
                    Guid = table.Column<byte[]>(type: "varbinary(16)", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Name);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RateToUsd");

            migrationBuilder.DropTable(
                name: "UserActions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
