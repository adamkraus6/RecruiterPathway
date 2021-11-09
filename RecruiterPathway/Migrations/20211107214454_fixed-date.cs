using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruiterPathway.Migrations
{
    public partial class fixeddate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Watch_AspNetUsers_RecruiterId",
                table: "Watch");

            migrationBuilder.DropForeignKey(
                name: "FK_Watch_Student_StudentId",
                table: "Watch");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Watch",
                table: "Watch");

            migrationBuilder.RenameTable(
                name: "Watch",
                newName: "WatchList");

            migrationBuilder.RenameIndex(
                name: "IX_Watch_StudentId",
                table: "WatchList",
                newName: "IX_WatchList_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Watch_RecruiterId",
                table: "WatchList",
                newName: "IX_WatchList_RecruiterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WatchList",
                table: "WatchList",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WatchList_AspNetUsers_RecruiterId",
                table: "WatchList",
                column: "RecruiterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchList_Student_StudentId",
                table: "WatchList",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WatchList_AspNetUsers_RecruiterId",
                table: "WatchList");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchList_Student_StudentId",
                table: "WatchList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WatchList",
                table: "WatchList");

            migrationBuilder.RenameTable(
                name: "WatchList",
                newName: "Watch");

            migrationBuilder.RenameIndex(
                name: "IX_WatchList_StudentId",
                table: "Watch",
                newName: "IX_Watch_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_WatchList_RecruiterId",
                table: "Watch",
                newName: "IX_Watch_RecruiterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Watch",
                table: "Watch",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Watch_AspNetUsers_RecruiterId",
                table: "Watch",
                column: "RecruiterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Watch_Student_StudentId",
                table: "Watch",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
