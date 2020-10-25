using Microsoft.EntityFrameworkCore.Migrations;

namespace GunplaOnlineShop.Data.Migrations
{
    public partial class aa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailingAddresses_AspNetUsers_ApplicationUserId",
                table: "MailingAddresses");

            migrationBuilder.DropIndex(
                name: "IX_MailingAddresses_ApplicationUserId",
                table: "MailingAddresses");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "MailingAddresses");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "MailingAddresses",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MailingAddresses_CustomerId",
                table: "MailingAddresses",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MailingAddresses_AspNetUsers_CustomerId",
                table: "MailingAddresses",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailingAddresses_AspNetUsers_CustomerId",
                table: "MailingAddresses");

            migrationBuilder.DropIndex(
                name: "IX_MailingAddresses_CustomerId",
                table: "MailingAddresses");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "MailingAddresses");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "MailingAddresses",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MailingAddresses_ApplicationUserId",
                table: "MailingAddresses",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MailingAddresses_AspNetUsers_ApplicationUserId",
                table: "MailingAddresses",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
