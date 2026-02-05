using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EChamado.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGridifyIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StatusType_CreatedAt",
                schema: "public",
                table: "StatusType",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StatusType_IsDeleted",
                schema: "public",
                table: "StatusType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_StatusType_IsDeleted_Name",
                schema: "public",
                table: "StatusType",
                columns: new[] { "IsDeleted", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_StatusType_Name",
                schema: "public",
                table: "StatusType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrderType_CreatedAt",
                schema: "public",
                table: "OrderType",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrderType_IsDeleted",
                schema: "public",
                table: "OrderType",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderType_IsDeleted_Name",
                schema: "public",
                table: "OrderType",
                columns: new[] { "IsDeleted", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderType_Name",
                schema: "public",
                table: "OrderType",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ClosingDate",
                schema: "public",
                table: "Order",
                column: "ClosingDate");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedAt",
                schema: "public",
                table: "Order",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DueDate",
                schema: "public",
                table: "Order",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IsDeleted",
                schema: "public",
                table: "Order",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IsDeleted_StatusId_CreatedAt",
                schema: "public",
                table: "Order",
                columns: new[] { "IsDeleted", "StatusId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Order_OpeningDate",
                schema: "public",
                table: "Order",
                column: "OpeningDate");

            migrationBuilder.CreateIndex(
                name: "IX_Order_RequestingUserId",
                schema: "public",
                table: "Order",
                column: "RequestingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ResponsibleUserId",
                schema: "public",
                table: "Order",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CreatedAt",
                schema: "public",
                table: "Department",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Department_IsDeleted",
                schema: "public",
                table: "Department",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Department_IsDeleted_Name",
                schema: "public",
                table: "Department",
                columns: new[] { "IsDeleted", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Department_Name",
                schema: "public",
                table: "Department",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CreatedAt",
                schema: "public",
                table: "Category",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Category_IsDeleted",
                schema: "public",
                table: "Category",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Category_IsDeleted_Name",
                schema: "public",
                table: "Category",
                columns: new[] { "IsDeleted", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                schema: "public",
                table: "Category",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StatusType_CreatedAt",
                schema: "public",
                table: "StatusType");

            migrationBuilder.DropIndex(
                name: "IX_StatusType_IsDeleted",
                schema: "public",
                table: "StatusType");

            migrationBuilder.DropIndex(
                name: "IX_StatusType_IsDeleted_Name",
                schema: "public",
                table: "StatusType");

            migrationBuilder.DropIndex(
                name: "IX_StatusType_Name",
                schema: "public",
                table: "StatusType");

            migrationBuilder.DropIndex(
                name: "IX_OrderType_CreatedAt",
                schema: "public",
                table: "OrderType");

            migrationBuilder.DropIndex(
                name: "IX_OrderType_IsDeleted",
                schema: "public",
                table: "OrderType");

            migrationBuilder.DropIndex(
                name: "IX_OrderType_IsDeleted_Name",
                schema: "public",
                table: "OrderType");

            migrationBuilder.DropIndex(
                name: "IX_OrderType_Name",
                schema: "public",
                table: "OrderType");

            migrationBuilder.DropIndex(
                name: "IX_Order_ClosingDate",
                schema: "public",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_CreatedAt",
                schema: "public",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_DueDate",
                schema: "public",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_IsDeleted",
                schema: "public",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_IsDeleted_StatusId_CreatedAt",
                schema: "public",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_OpeningDate",
                schema: "public",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_RequestingUserId",
                schema: "public",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ResponsibleUserId",
                schema: "public",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Department_CreatedAt",
                schema: "public",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Department_IsDeleted",
                schema: "public",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Department_IsDeleted_Name",
                schema: "public",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Department_Name",
                schema: "public",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Category_CreatedAt",
                schema: "public",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_IsDeleted",
                schema: "public",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_IsDeleted_Name",
                schema: "public",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_Name",
                schema: "public",
                table: "Category");
        }
    }
}
