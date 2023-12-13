using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteQuanLyLamViecNhom.Data.Migrations
{
    public partial class ClassModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Group = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectRequirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleGroup = table.Column<int>(type: "int", nullable: false),
                    RoleProject = table.Column<int>(type: "int", nullable: false),
                    OpenDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    TeacherId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Class_AspNetUsers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Class_TeacherId",
                table: "Class",
                column: "TeacherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Class");
        }
    }
}
