using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddedCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovieCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCollection", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "CollectionId",
                table: "Movie",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movie_CollectionId",
                table: "Movie",
                column: "CollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movie_MovieCollection_CollectionId",
                table: "Movie",
                column: "CollectionId",
                principalTable: "MovieCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movie_MovieCollection_CollectionId",
                table: "Movie");

            migrationBuilder.DropTable(
                name: "MovieCollection");

            migrationBuilder.DropIndex(
                name: "IX_Movie_CollectionId",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Movie");
        }
    }
}
