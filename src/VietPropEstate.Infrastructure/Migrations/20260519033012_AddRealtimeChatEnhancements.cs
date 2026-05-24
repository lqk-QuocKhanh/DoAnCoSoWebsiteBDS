using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietPropEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRealtimeChatEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuyerUnreadCount",
                table: "Conversations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMessageAt",
                table: "Conversations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastMessagePreview",
                table: "Conversations",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SellerUnreadCount",
                table: "Conversations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_LastMessageAt",
                table: "Conversations",
                column: "LastMessageAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conversations_LastMessageAt",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "BuyerUnreadCount",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "LastMessageAt",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "LastMessagePreview",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SellerUnreadCount",
                table: "Conversations");
        }
    }
}
