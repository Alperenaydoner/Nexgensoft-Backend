using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoreService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobPositionsTableAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "job_positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_positions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "job_positions",
                columns: new[] { "Id", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("3c26d21f-ba4d-4e8a-9c89-296a6c0a0381"), true, "Yazilim Gelistirici", 30 },
                    { new Guid("537da80b-3fde-4ec0-9946-84d2efd2d214"), true, "Sofor", 20 },
                    { new Guid("5fcba5fd-2c26-4634-a8a4-c9892a2a2a11"), true, "Asistan", 10 },
                    { new Guid("8dc3a70a-5719-4ee4-9a95-9d471c61139e"), true, "DevOps Muhendisi", 50 },
                    { new Guid("b089a454-5d9b-4267-84c4-bb3f02579d88"), true, "Frontend Gelistirici", 40 },
                    { new Guid("c2af8e5e-c91d-4ea2-a34e-75257d7fb016"), true, "Stajyer", 60 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_job_positions_Name",
                table: "job_positions",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_positions");
        }
    }
}
