using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VietPropEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVnPayPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GatewayResponse",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Payments",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderInfo",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "VIPPackageId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VnpayTransactionId",
                table: "Payments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.InsertData(
                table: "VIPPackages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DurationDays", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "MaxListings", "Name", "PriceAmount", "PriceCurrency" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Đăng tối đa 3 tin rao trong 30 ngày. Phù hợp cho cá nhân.", 30, true, false, null, null, 3, "Cơ Bản", 500000m, "VND" },
                    { new Guid("60000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Đăng tối đa 10 tin rao trong 30 ngày. Phù hợp cho môi giới cá nhân.", 30, true, false, null, null, 10, "Tiêu Chuẩn", 1000000m, "VND" },
                    { new Guid("60000000-0000-0000-0000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Đăng tối đa 30 tin rao trong 90 ngày. Ưu tiên hiển thị trên trang chủ.", 90, true, false, null, null, 30, "Cao Cấp", 2500000m, "VND" },
                    { new Guid("60000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "Không giới hạn tin rao trong 180 ngày. Được gắn nhãn VIP nổi bật.", 180, true, false, null, null, 99, "Gold", 5000000m, "VND" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentReference",
                table: "Payments",
                column: "PaymentReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_VIPPackageId",
                table: "Payments",
                column: "VIPPackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentReference",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_VIPPackageId",
                table: "Payments");

            migrationBuilder.DeleteData(
                table: "VIPPackages",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "VIPPackages",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "VIPPackages",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "VIPPackages",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000004"));

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderInfo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "VIPPackageId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "VnpayTransactionId",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "GatewayResponse",
                table: "Payments",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
