using Microsoft.EntityFrameworkCore.Migrations;

namespace GunplaOnlineShop.Data.Migrations
{
    public partial class photoModelChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Photos",
                type: "LONGTEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Photos");
        }
    }
}
