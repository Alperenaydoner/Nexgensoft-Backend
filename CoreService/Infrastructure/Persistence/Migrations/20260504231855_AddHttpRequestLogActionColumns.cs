using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHttpRequestLogActionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionDescription",
                table: "http_request_logs",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionTitle",
                table: "http_request_logs",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "http_request_logs",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_http_request_logs_ActionType",
                table: "http_request_logs",
                column: "ActionType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_http_request_logs_ActionType",
                table: "http_request_logs");

            migrationBuilder.DropColumn(
                name: "ActionDescription",
                table: "http_request_logs");

            migrationBuilder.DropColumn(
                name: "ActionTitle",
                table: "http_request_logs");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "http_request_logs");
        }
    }
}
