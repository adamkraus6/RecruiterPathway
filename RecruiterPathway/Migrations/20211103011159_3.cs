using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruiterPathway.Migrations
{
    public partial class _3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PipelineStatus_AspNetUsers_RecruiterId",
                table: "PipelineStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_PipelineStatus_AspNetUsers_RecruiterId",
                table: "PipelineStatus",
                column: "RecruiterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PipelineStatus_AspNetUsers_RecruiterId",
                table: "PipelineStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_PipelineStatus_AspNetUsers_RecruiterId",
                table: "PipelineStatus",
                column: "RecruiterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
