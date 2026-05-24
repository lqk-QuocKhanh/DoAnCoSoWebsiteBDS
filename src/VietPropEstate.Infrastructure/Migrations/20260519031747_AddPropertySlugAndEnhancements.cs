using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietPropEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertySlugAndEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Properties",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Slug",
                table: "Properties",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Properties_Slug",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Properties");
        }
    }
}
