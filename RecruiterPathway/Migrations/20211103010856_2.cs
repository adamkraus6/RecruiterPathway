using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruiterPathway.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Student_StudentId",
                table: "Comment");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Student_StudentId",
                table: "Comment",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Student_StudentId",
                table: "Comment");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Student_StudentId",
                table: "Comment",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
