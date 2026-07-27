using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EChamado.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameAuditColumnsToUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "SubCategory",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "public",
                table: "SubCategory",
                newName: "DeletedAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "SubCategory",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "StatusType",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "public",
                table: "StatusType",
                newName: "DeletedAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "StatusType",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_StatusType_CreatedAt",
                schema: "public",
                table: "StatusType",
                newName: "IX_StatusType_CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "OrderType",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "public",
                table: "OrderType",
                newName: "DeletedAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "OrderType",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_OrderType_CreatedAt",
                schema: "public",
                table: "OrderType",
                newName: "IX_OrderType_CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "Order",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "public",
                table: "Order",
                newName: "DeletedAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "Order",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_Order_IsDeleted_StatusId_CreatedAt",
                schema: "public",
                table: "Order",
                newName: "IX_Order_IsDeleted_StatusId_CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CreatedAt",
                schema: "public",
                table: "Order",
                newName: "IX_Order_CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "Department",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "public",
                table: "Department",
                newName: "DeletedAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "Department",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_Department_CreatedAt",
                schema: "public",
                table: "Department",
                newName: "IX_Department_CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "Comment",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "public",
                table: "Comment",
                newName: "DeletedAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "Comment",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "public",
                table: "Category",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "public",
                table: "Category",
                newName: "DeletedAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "Category",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_Category_CreatedAt",
                schema: "public",
                table: "Category",
                newName: "IX_Category_CreatedAtUtc");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OpeningDate",
                schema: "public",
                table: "Order",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CreatedAtUtc",
                schema: "public",
                table: "SubCategory",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_IsDeleted",
                schema: "public",
                table: "SubCategory",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_IsDeleted_Name",
                schema: "public",
                table: "SubCategory",
                columns: new[] { "IsDeleted", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_Name",
                schema: "public",
                table: "SubCategory",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_CreatedAtUtc",
                schema: "public",
                table: "Comment",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_IsDeleted",
                schema: "public",
                table: "Comment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                schema: "public",
                table: "Comment",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubCategory_CreatedAtUtc",
                schema: "public",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_SubCategory_IsDeleted",
                schema: "public",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_SubCategory_IsDeleted_Name",
                schema: "public",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_SubCategory_Name",
                schema: "public",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_Comment_CreatedAtUtc",
                schema: "public",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_IsDeleted",
                schema: "public",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_UserId",
                schema: "public",
                table: "Comment");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "SubCategory",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "SubCategory",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "SubCategory",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "StatusType",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "StatusType",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "StatusType",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_StatusType_CreatedAtUtc",
                schema: "public",
                table: "StatusType",
                newName: "IX_StatusType_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "OrderType",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "OrderType",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "OrderType",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OrderType_CreatedAtUtc",
                schema: "public",
                table: "OrderType",
                newName: "IX_OrderType_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "Order",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Order",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "Order",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Order_IsDeleted_StatusId_CreatedAtUtc",
                schema: "public",
                table: "Order",
                newName: "IX_Order_IsDeleted_StatusId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CreatedAtUtc",
                schema: "public",
                table: "Order",
                newName: "IX_Order_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "Department",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Department",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "Department",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Department_CreatedAtUtc",
                schema: "public",
                table: "Department",
                newName: "IX_Department_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "Comment",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Comment",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "Comment",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                schema: "public",
                table: "Category",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Category",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                schema: "public",
                table: "Category",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Category_CreatedAtUtc",
                schema: "public",
                table: "Category",
                newName: "IX_Category_CreatedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OpeningDate",
                schema: "public",
                table: "Order",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
