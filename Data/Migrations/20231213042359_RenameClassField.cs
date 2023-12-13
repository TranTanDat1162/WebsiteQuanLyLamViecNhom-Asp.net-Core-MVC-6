using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteQuanLyLamViecNhom.Data.Migrations
{
    public partial class RenameClassField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Group",
                table: "Class",
                newName: "MOTD");

            migrationBuilder.AddColumn<string>(
                name: "ClassGroup",
                table: "Class",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassGroup",
                table: "Class");

            migrationBuilder.RenameColumn(
                name: "MOTD",
                table: "Class",
                newName: "Group");
        }
    }
}
