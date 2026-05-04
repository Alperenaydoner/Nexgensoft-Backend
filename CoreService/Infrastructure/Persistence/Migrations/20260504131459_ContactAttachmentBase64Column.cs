using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContactAttachmentBase64Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoredFileName",
                table: "contact_attachments");

            migrationBuilder.AddColumn<string>(
                name: "ContentBase64",
                table: "contact_attachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentBase64",
                table: "contact_attachments");

            migrationBuilder.AddColumn<string>(
                name: "StoredFileName",
                table: "contact_attachments",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: false,
                defaultValue: "");
        }
    }
}
