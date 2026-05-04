using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteLocalizedStrings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "site_content_bundles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Locale = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_content_bundles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "site_localized_strings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Locale = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StringKey = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_localized_strings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_site_content_bundles_Locale",
                table: "site_content_bundles",
                column: "Locale",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_localized_strings_Locale_StringKey",
                table: "site_localized_strings",
                columns: new[] { "Locale", "StringKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "site_content_bundles");

            migrationBuilder.DropTable(
                name: "site_localized_strings");
        }
    }
}
