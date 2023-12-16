using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteQuanLyLamViecNhom.Data.Migrations
{
    public partial class CreateClassNStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClass_Group_GroupID",
                table: "StudentClass");

            migrationBuilder.AlterColumn<float>(
                name: "Score",
                table: "StudentClass",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<string>(
                name: "GroupID",
                table: "StudentClass",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClass_Group_GroupID",
                table: "StudentClass",
                column: "GroupID",
                principalTable: "Group",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClass_Group_GroupID",
                table: "StudentClass");

            migrationBuilder.AlterColumn<float>(
                name: "Score",
                table: "StudentClass",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GroupID",
                table: "StudentClass",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClass_Group_GroupID",
                table: "StudentClass",
                column: "GroupID",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
