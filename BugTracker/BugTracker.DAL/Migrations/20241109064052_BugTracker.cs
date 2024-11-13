using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracker.DAL.Migrations
{
    /// <inheritdoc />
    public partial class BugTracker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "TaskItemIdSequence",
                minValue: 1L,
                maxValue: 100000L);

            migrationBuilder.CreateTable(
                name: "TaskItem",
                columns: table => new
                {
                    TaskItemId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "NEXT VALUE FOR TaskItemIdSequence"),
                    Topic = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Readiness = table.Column<int>(type: "integer", nullable: false),
                    AffectedVersion = table.Column<string>(type: "text", nullable: false),
                    Files = table.Column<byte[]>(type: "bytea", nullable: false),
                    TaskItemVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItem", x => x.TaskItemId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskItem");

            migrationBuilder.DropSequence(
                name: "TaskItemIdSequence");
        }
    }
}
