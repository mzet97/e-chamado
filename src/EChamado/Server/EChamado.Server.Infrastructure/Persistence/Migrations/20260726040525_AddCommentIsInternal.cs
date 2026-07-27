using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EChamado.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentIsInternal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                schema: "public",
                table: "Comment",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInternal",
                schema: "public",
                table: "Comment");
        }
    }
}
