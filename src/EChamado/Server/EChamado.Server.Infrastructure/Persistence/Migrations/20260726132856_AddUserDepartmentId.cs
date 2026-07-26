using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EChamado.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDepartmentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                schema: "public",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "public",
                table: "AspNetUsers");
        }
    }
}
