using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruiterPathway.Migrations
{
    public partial class watch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Student_StudentId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_PipelineStatus_AspNetUsers_RecruiterId",
                table: "PipelineStatus");

            migrationBuilder.DropColumn(
                name: "WatchList",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "PipelineStatus",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecruiterId",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Watch",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RecruiterId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Watch_AspNetUsers_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Watch_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStatus_StudentId",
                table: "PipelineStatus",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_RecruiterId",
                table: "Comment",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_Watch_RecruiterId",
                table: "Watch",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_Watch_StudentId",
                table: "Watch",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_RecruiterId",
                table: "Comment",
                column: "RecruiterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Student_StudentId",
                table: "Comment",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PipelineStatus_AspNetUsers_RecruiterId",
                table: "PipelineStatus",
                column: "RecruiterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PipelineStatus_Student_StudentId",
                table: "PipelineStatus",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_RecruiterId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Student_StudentId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_PipelineStatus_AspNetUsers_RecruiterId",
                table: "PipelineStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_PipelineStatus_Student_StudentId",
                table: "PipelineStatus");

            migrationBuilder.DropTable(
                name: "Watch");

            migrationBuilder.DropIndex(
                name: "IX_PipelineStatus_StudentId",
                table: "PipelineStatus");

            migrationBuilder.DropIndex(
                name: "IX_Comment_RecruiterId",
                table: "Comment");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "PipelineStatus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecruiterId",
                table: "Comment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WatchList",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Student_StudentId",
                table: "Comment",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PipelineStatus_AspNetUsers_RecruiterId",
                table: "PipelineStatus",
                column: "RecruiterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
