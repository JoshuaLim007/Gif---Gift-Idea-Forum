using Microsoft.EntityFrameworkCore.Migrations;

namespace GIF_GIftIdeaForum.Migrations
{
    public partial class AddImageUriToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURI",
                table: "PresentIdeas",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURI",
                table: "PresentIdeas");
        }
    }
}
