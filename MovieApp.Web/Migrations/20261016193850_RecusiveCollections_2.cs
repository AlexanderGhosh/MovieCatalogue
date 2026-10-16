using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class RecusiveCollections_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Movies");

            migrationBuilder.AddColumn<bool>(
                name: "ShowThumbnail",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowThumbnail",
                table: "Movies");

            migrationBuilder.AddColumn<int>(
                name: "CollectionId",
                table: "Movies",
                type: "int",
                nullable: true);
        }
    }
}
