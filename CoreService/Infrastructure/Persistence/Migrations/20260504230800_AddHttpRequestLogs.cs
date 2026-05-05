using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHttpRequestLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "http_request_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    UserEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    UserRoles = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    HttpMethod = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Path = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    QueryString = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ClientIp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    AcceptLanguage = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Referer = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TraceId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EnvironmentName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    EndpointController = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EndpointAction = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    ExceptionType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExceptionMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RequestBodySnippet = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_http_request_logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_http_request_logs_ClientIp",
                table: "http_request_logs",
                column: "ClientIp");

            migrationBuilder.CreateIndex(
                name: "IX_http_request_logs_OccurredAtUtc",
                table: "http_request_logs",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_http_request_logs_Path_OccurredAtUtc",
                table: "http_request_logs",
                columns: new[] { "Path", "OccurredAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "http_request_logs");
        }
    }
}
