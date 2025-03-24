using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogPostApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHistoryKeyInUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameHistory",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameHistory",
                table: "Users");
        }
    }
}
