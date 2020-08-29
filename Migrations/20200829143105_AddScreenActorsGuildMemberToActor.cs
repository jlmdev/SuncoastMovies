using Microsoft.EntityFrameworkCore.Migrations;

namespace SuncoastMovies.Migrations
{
    public partial class AddScreenActorsGuildMemberToActor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ScreenActorsGuildMember",
                table: "Actors",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScreenActorsGuildMember",
                table: "Actors");
        }
    }
}
