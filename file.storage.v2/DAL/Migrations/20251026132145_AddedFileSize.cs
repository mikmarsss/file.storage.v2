using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace file.storage.v2.Migrations
{
    /// <inheritdoc />
    public partial class AddedFileSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Size",
                table: "UserFiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "UserFiles");
        }
    }
}
