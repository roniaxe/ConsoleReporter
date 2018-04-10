using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReporterConsole.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Defects",
                columns: table => new
                {
                    Desc = table.Column<string>(nullable: false),
                    BatchId = table.Column<int>(nullable: false),
                    TaskId = table.Column<int>(nullable: false),
                    DefectNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defects", x => new { x.Desc, x.BatchId, x.TaskId });
                    table.UniqueConstraint("AK_Defects_BatchId_Desc_TaskId", x => new { x.BatchId, x.Desc, x.TaskId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Defects");
        }
    }
}
