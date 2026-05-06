using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExtendBaseEntityAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "site_localized_strings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "site_localized_strings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "site_localized_strings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "site_localized_strings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "site_localized_strings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "site_content_bundles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "site_content_bundles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "site_content_bundles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "site_content_bundles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "site_content_bundles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "job_positions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "job_positions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "job_positions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "job_positions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "job_applications",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "job_applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "job_applications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "job_applications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "job_application_attachments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "job_application_attachments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "job_application_attachments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "job_application_attachments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "job_application_attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE job_application_attachments AS a
                SET "CreatedAtUtc" = j."CreatedAtUtc"
                FROM job_applications AS j
                WHERE j."Id" = a."JobApplicationId"
                """);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "contact_messages",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "contact_messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "contact_messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "contact_messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "contact_attachments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "contact_attachments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "contact_attachments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "contact_attachments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "contact_attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE contact_attachments AS a
                SET "CreatedAtUtc" = m."CreatedAtUtc"
                FROM contact_messages AS m
                WHERE m."Id" = a."ContactMessageId"
                """);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "app_users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "app_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "app_user_roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "app_user_roles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "app_user_roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "app_user_roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "app_roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "app_roles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "app_roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "app_roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "app_roles",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "job_positions",
                keyColumn: "Id",
                keyValue: new Guid("3c26d21f-ba4d-4e8a-9c89-296a6c0a0381"),
                columns: new[] { "CreatedAtUtc", "IsDeleted", "UpdatedAtUtc", "UserId" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null });

            migrationBuilder.UpdateData(
                table: "job_positions",
                keyColumn: "Id",
                keyValue: new Guid("537da80b-3fde-4ec0-9946-84d2efd2d214"),
                columns: new[] { "CreatedAtUtc", "IsDeleted", "UpdatedAtUtc", "UserId" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null });

            migrationBuilder.UpdateData(
                table: "job_positions",
                keyColumn: "Id",
                keyValue: new Guid("5fcba5fd-2c26-4634-a8a4-c9892a2a2a11"),
                columns: new[] { "CreatedAtUtc", "IsDeleted", "UpdatedAtUtc", "UserId" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null });

            migrationBuilder.UpdateData(
                table: "job_positions",
                keyColumn: "Id",
                keyValue: new Guid("8dc3a70a-5719-4ee4-9a95-9d471c61139e"),
                columns: new[] { "CreatedAtUtc", "IsDeleted", "UpdatedAtUtc", "UserId" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null });

            migrationBuilder.UpdateData(
                table: "job_positions",
                keyColumn: "Id",
                keyValue: new Guid("b089a454-5d9b-4267-84c4-bb3f02579d88"),
                columns: new[] { "CreatedAtUtc", "IsDeleted", "UpdatedAtUtc", "UserId" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null });

            migrationBuilder.UpdateData(
                table: "job_positions",
                keyColumn: "Id",
                keyValue: new Guid("c2af8e5e-c91d-4ea2-a34e-75257d7fb016"),
                columns: new[] { "CreatedAtUtc", "IsDeleted", "UpdatedAtUtc", "UserId" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_site_localized_strings_IsActive_IsDeleted",
                table: "site_localized_strings",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_site_localized_strings_UserId",
                table: "site_localized_strings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_site_content_bundles_IsActive_IsDeleted",
                table: "site_content_bundles",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_site_content_bundles_UserId",
                table: "site_content_bundles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_job_positions_IsActive_IsDeleted",
                table: "job_positions",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_job_positions_IsActive_SortOrder",
                table: "job_positions",
                columns: new[] { "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_job_positions_UserId",
                table: "job_positions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_CreatedAtUtc",
                table: "job_applications",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_Email",
                table: "job_applications",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_IsActive_IsDeleted",
                table: "job_applications",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_Position",
                table: "job_applications",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_Position_CreatedAtUtc",
                table: "job_applications",
                columns: new[] { "Position", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_UserId",
                table: "job_applications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_job_application_attachments_IsActive_IsDeleted",
                table: "job_application_attachments",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_job_application_attachments_UserId",
                table: "job_application_attachments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_http_request_logs_HttpMethod",
                table: "http_request_logs",
                column: "HttpMethod");

            migrationBuilder.CreateIndex(
                name: "IX_http_request_logs_HttpMethod_OccurredAtUtc",
                table: "http_request_logs",
                columns: new[] { "HttpMethod", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_http_request_logs_StatusCode",
                table: "http_request_logs",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_http_request_logs_StatusCode_OccurredAtUtc",
                table: "http_request_logs",
                columns: new[] { "StatusCode", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_contact_messages_CreatedAtUtc",
                table: "contact_messages",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_contact_messages_Email",
                table: "contact_messages",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_contact_messages_IsActive_IsDeleted",
                table: "contact_messages",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_contact_messages_UserId",
                table: "contact_messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_contact_attachments_IsActive_IsDeleted",
                table: "contact_attachments",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_contact_attachments_UserId",
                table: "contact_attachments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_app_users_CreatedAtUtc",
                table: "app_users",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_app_users_IsActive_IsDeleted",
                table: "app_users",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_app_users_UserId",
                table: "app_users",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_app_user_roles_IsActive_IsDeleted",
                table: "app_user_roles",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_app_roles_IsActive_IsDeleted",
                table: "app_roles",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_app_roles_Name",
                table: "app_roles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_app_roles_UserId",
                table: "app_roles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_app_roles_app_users_UserId",
                table: "app_roles",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_contact_attachments_app_users_UserId",
                table: "contact_attachments",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_contact_messages_app_users_UserId",
                table: "contact_messages",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_job_application_attachments_app_users_UserId",
                table: "job_application_attachments",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_job_applications_app_users_UserId",
                table: "job_applications",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_job_positions_app_users_UserId",
                table: "job_positions",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_site_content_bundles_app_users_UserId",
                table: "site_content_bundles",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_site_localized_strings_app_users_UserId",
                table: "site_localized_strings",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_app_roles_app_users_UserId",
                table: "app_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_contact_attachments_app_users_UserId",
                table: "contact_attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_contact_messages_app_users_UserId",
                table: "contact_messages");

            migrationBuilder.DropForeignKey(
                name: "FK_job_application_attachments_app_users_UserId",
                table: "job_application_attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_job_applications_app_users_UserId",
                table: "job_applications");

            migrationBuilder.DropForeignKey(
                name: "FK_job_positions_app_users_UserId",
                table: "job_positions");

            migrationBuilder.DropForeignKey(
                name: "FK_site_content_bundles_app_users_UserId",
                table: "site_content_bundles");

            migrationBuilder.DropForeignKey(
                name: "FK_site_localized_strings_app_users_UserId",
                table: "site_localized_strings");

            migrationBuilder.DropIndex(
                name: "IX_site_localized_strings_IsActive_IsDeleted",
                table: "site_localized_strings");

            migrationBuilder.DropIndex(
                name: "IX_site_localized_strings_UserId",
                table: "site_localized_strings");

            migrationBuilder.DropIndex(
                name: "IX_site_content_bundles_IsActive_IsDeleted",
                table: "site_content_bundles");

            migrationBuilder.DropIndex(
                name: "IX_site_content_bundles_UserId",
                table: "site_content_bundles");

            migrationBuilder.DropIndex(
                name: "IX_job_positions_IsActive_IsDeleted",
                table: "job_positions");

            migrationBuilder.DropIndex(
                name: "IX_job_positions_IsActive_SortOrder",
                table: "job_positions");

            migrationBuilder.DropIndex(
                name: "IX_job_positions_UserId",
                table: "job_positions");

            migrationBuilder.DropIndex(
                name: "IX_job_applications_CreatedAtUtc",
                table: "job_applications");

            migrationBuilder.DropIndex(
                name: "IX_job_applications_Email",
                table: "job_applications");

            migrationBuilder.DropIndex(
                name: "IX_job_applications_IsActive_IsDeleted",
                table: "job_applications");

            migrationBuilder.DropIndex(
                name: "IX_job_applications_Position",
                table: "job_applications");

            migrationBuilder.DropIndex(
                name: "IX_job_applications_Position_CreatedAtUtc",
                table: "job_applications");

            migrationBuilder.DropIndex(
                name: "IX_job_applications_UserId",
                table: "job_applications");

            migrationBuilder.DropIndex(
                name: "IX_job_application_attachments_IsActive_IsDeleted",
                table: "job_application_attachments");

            migrationBuilder.DropIndex(
                name: "IX_job_application_attachments_UserId",
                table: "job_application_attachments");

            migrationBuilder.DropIndex(
                name: "IX_http_request_logs_HttpMethod",
                table: "http_request_logs");

            migrationBuilder.DropIndex(
                name: "IX_http_request_logs_HttpMethod_OccurredAtUtc",
                table: "http_request_logs");

            migrationBuilder.DropIndex(
                name: "IX_http_request_logs_StatusCode",
                table: "http_request_logs");

            migrationBuilder.DropIndex(
                name: "IX_http_request_logs_StatusCode_OccurredAtUtc",
                table: "http_request_logs");

            migrationBuilder.DropIndex(
                name: "IX_contact_messages_CreatedAtUtc",
                table: "contact_messages");

            migrationBuilder.DropIndex(
                name: "IX_contact_messages_Email",
                table: "contact_messages");

            migrationBuilder.DropIndex(
                name: "IX_contact_messages_IsActive_IsDeleted",
                table: "contact_messages");

            migrationBuilder.DropIndex(
                name: "IX_contact_messages_UserId",
                table: "contact_messages");

            migrationBuilder.DropIndex(
                name: "IX_contact_attachments_IsActive_IsDeleted",
                table: "contact_attachments");

            migrationBuilder.DropIndex(
                name: "IX_contact_attachments_UserId",
                table: "contact_attachments");

            migrationBuilder.DropIndex(
                name: "IX_app_users_CreatedAtUtc",
                table: "app_users");

            migrationBuilder.DropIndex(
                name: "IX_app_users_IsActive_IsDeleted",
                table: "app_users");

            migrationBuilder.DropIndex(
                name: "IX_app_users_UserId",
                table: "app_users");

            migrationBuilder.DropIndex(
                name: "IX_app_user_roles_IsActive_IsDeleted",
                table: "app_user_roles");

            migrationBuilder.DropIndex(
                name: "IX_app_roles_IsActive_IsDeleted",
                table: "app_roles");

            migrationBuilder.DropIndex(
                name: "IX_app_roles_Name",
                table: "app_roles");

            migrationBuilder.DropIndex(
                name: "IX_app_roles_UserId",
                table: "app_roles");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "site_localized_strings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "site_localized_strings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "site_localized_strings");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "site_localized_strings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "site_localized_strings");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "site_content_bundles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "site_content_bundles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "site_content_bundles");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "site_content_bundles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "site_content_bundles");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "job_positions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "job_positions");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "job_positions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "job_positions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "job_application_attachments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "job_application_attachments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "job_application_attachments");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "job_application_attachments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "job_application_attachments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "contact_messages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "contact_messages");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "contact_messages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "contact_messages");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "contact_attachments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "contact_attachments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "contact_attachments");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "contact_attachments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "contact_attachments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "app_user_roles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "app_user_roles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "app_user_roles");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "app_user_roles");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "app_roles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "app_roles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "app_roles");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "app_roles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "app_roles");
        }
    }
}
