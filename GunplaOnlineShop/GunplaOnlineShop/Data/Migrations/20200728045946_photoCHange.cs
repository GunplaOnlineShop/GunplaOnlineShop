using Microsoft.EntityFrameworkCore.Migrations;

namespace GunplaOnlineShop.Data.Migrations
{
    public partial class photoCHange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Photos",
                newName: "URL");

            migrationBuilder.AlterColumn<string>(
                name: "URL",
                table: "Photos",
                type: "LONGTEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100) CHARACTER SET utf8mb4",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Photos",
                type: "LONGTEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(25) CHARACTER SET utf8mb4",
                oldMaxLength: 25);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "URL",
                table: "Photos",
                newName: "Url");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Photos",
                type: "varchar(100) CHARACTER SET utf8mb4",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "LONGTEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Photos",
                type: "varchar(25) CHARACTER SET utf8mb4",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "LONGTEXT");
        }
    }
}
