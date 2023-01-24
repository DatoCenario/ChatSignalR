using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.EF.Migrations
{
    public partial class NewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InviterId",
                table: "ChatUsers",
                type: "bigint",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SendTime",
                table: "ChatMessages",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_InviterId",
                table: "ChatUsers",
                column: "InviterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_AspNetUsers_InviterId",
                table: "ChatUsers",
                column: "InviterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_AspNetUsers_InviterId",
                table: "ChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChatUsers_InviterId",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "InviterId",
                table: "ChatUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SendTime",
                table: "ChatMessages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
