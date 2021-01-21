using Microsoft.EntityFrameworkCore.Migrations;

namespace MapApi.Migrations
{
    public partial class FixTableNamesForReal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_location_game_GameId",
                table: "location");

            migrationBuilder.DropPrimaryKey(
                name: "PK_location",
                table: "location");

            migrationBuilder.DropPrimaryKey(
                name: "PK_game",
                table: "game");

            migrationBuilder.RenameTable(
                name: "location",
                newName: "locations");

            migrationBuilder.RenameTable(
                name: "game",
                newName: "games");

            migrationBuilder.RenameIndex(
                name: "IX_location_GameId",
                table: "locations",
                newName: "IX_locations_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_locations",
                table: "locations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_games",
                table: "games",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_locations_games_GameId",
                table: "locations",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_locations_games_GameId",
                table: "locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_locations",
                table: "locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_games",
                table: "games");

            migrationBuilder.RenameTable(
                name: "locations",
                newName: "location");

            migrationBuilder.RenameTable(
                name: "games",
                newName: "game");

            migrationBuilder.RenameIndex(
                name: "IX_locations_GameId",
                table: "location",
                newName: "IX_location_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_location",
                table: "location",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_game",
                table: "game",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_location_game_GameId",
                table: "location",
                column: "GameId",
                principalTable: "game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
