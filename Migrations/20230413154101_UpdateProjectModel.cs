using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Migrations
{
    public partial class UpdateProjectModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MangerId",
                table: "Project",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Project_MangerId",
                table: "Project",
                column: "MangerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_MangerId",
                table: "Project",
                column: "MangerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_MangerId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_MangerId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "MangerId",
                table: "Project");
        }
    }
}
