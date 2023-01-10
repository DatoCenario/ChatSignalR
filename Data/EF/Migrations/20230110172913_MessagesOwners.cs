using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class MessagesOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OwnerId",
                table: "ChatMessages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_OwnerId",
                table: "ChatMessages",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_AspNetUsers_OwnerId",
                table: "ChatMessages",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_AspNetUsers_OwnerId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_OwnerId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ChatMessages");
        }
    }
}
