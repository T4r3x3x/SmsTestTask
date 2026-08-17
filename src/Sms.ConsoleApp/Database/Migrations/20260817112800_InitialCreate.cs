using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sms.ConsoleApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "menu_items",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    article = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    is_weighted = table.Column<bool>(type: "boolean", nullable: false),
                    full_path = table.Column<string>(type: "text", nullable: false),
                    barcodes = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_items", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "menu_items");
        }
    }
}
