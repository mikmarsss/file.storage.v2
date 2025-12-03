using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace file.storage.v2.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Loggings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Loggings_UserId",
                table: "Loggings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loggings_Users_UserId",
                table: "Loggings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loggings_Users_UserId",
                table: "Loggings");

            migrationBuilder.DropIndex(
                name: "IX_Loggings_UserId",
                table: "Loggings");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Loggings");
        }
    }
}
