using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace file.storage.v2.Migrations
{
    /// <inheritdoc />
    public partial class RemovedNewFieldInLoggingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionStatus",
                table: "Loggings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionStatus",
                table: "Loggings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
