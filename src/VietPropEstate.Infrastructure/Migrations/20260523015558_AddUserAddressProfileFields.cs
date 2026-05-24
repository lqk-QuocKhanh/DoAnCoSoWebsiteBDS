using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietPropEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAddressProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceCode",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardCode",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProvinceName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WardName",
                table: "AspNetUsers");
        }
    }
}
