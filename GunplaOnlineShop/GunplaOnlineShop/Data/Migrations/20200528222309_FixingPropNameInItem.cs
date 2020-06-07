using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GunplaOnlineShop.Data.Migrations
{
    public partial class FixingPropNameInItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemIsAvailable",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemReleaseDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemTotalSales",
                table: "Items");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Items",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Items",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TotalSales",
                table: "Items",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "TotalSales",
                table: "Items");

            migrationBuilder.AddColumn<bool>(
                name: "ItemIsAvailable",
                table: "Items",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ItemReleaseDate",
                table: "Items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ItemTotalSales",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
