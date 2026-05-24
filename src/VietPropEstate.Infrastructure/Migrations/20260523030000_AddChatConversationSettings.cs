using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietPropEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChatConversationSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    BlockedUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBlocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserConversationSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPinned = table.Column<bool>(type: "bit", nullable: false),
                    IsMuted = table.Column<bool>(type: "bit", nullable: false),
                    PinnedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConversationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConversationSettings_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockedUserId",
                table: "UserBlocks",
                column: "BlockedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockerId_BlockedUserId",
                table: "UserBlocks",
                columns: new[] { "BlockerId", "BlockedUserId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UserConversationSettings_ConversationId",
                table: "UserConversationSettings",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConversationSettings_UserId_ConversationId",
                table: "UserConversationSettings",
                columns: new[] { "UserId", "ConversationId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UserConversationSettings_UserId_IsPinned",
                table: "UserConversationSettings",
                columns: new[] { "UserId", "IsPinned" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserConversationSettings");
            migrationBuilder.DropTable(name: "UserBlocks");
        }
    }
}
