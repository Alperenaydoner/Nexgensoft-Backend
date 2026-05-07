using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNewIndexAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_job_application_attachments_JobApplicationId_CreatedAtUtc",
                table: "job_application_attachments",
                columns: new[] { "JobApplicationId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_contact_attachments_ContactMessageId_CreatedAtUtc",
                table: "contact_attachments",
                columns: new[] { "ContactMessageId", "CreatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_job_application_attachments_JobApplicationId_CreatedAtUtc",
                table: "job_application_attachments");

            migrationBuilder.DropIndex(
                name: "IX_contact_attachments_ContactMessageId_CreatedAtUtc",
                table: "contact_attachments");
        }
    }
}
