using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class RecusiveCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movie_MovieCollection_CollectionId",
                table: "Movie");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieContent_Movie_MovieId",
                table: "MovieContent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieCollection",
                table: "MovieCollection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movie",
                table: "Movie");

            migrationBuilder.DropIndex(
                name: "IX_Movie_CollectionId",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "LastTimeStamp",
                table: "MovieContent");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "MovieContent");

            migrationBuilder.RenameTable(
                name: "MovieCollection",
                newName: "Collections");

            migrationBuilder.RenameTable(
                name: "Movie",
                newName: "Movies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Collections",
                table: "Collections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movies",
                table: "Movies",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Collection_Collection",
                columns: table => new
                {
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    ChildId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collection_Collection", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_Collection_Collection_Collections_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Collection_Collection_Collections_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Movie_Collection",
                columns: table => new
                {
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie_Collection", x => new { x.MovieId, x.CollectionId });
                    table.ForeignKey(
                        name: "FK_Movie_Collection_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movie_Collection_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collection_Collection_ChildId",
                table: "Collection_Collection",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Movie_Collection_CollectionId",
                table: "Movie_Collection",
                column: "CollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieContent_Movies_MovieId",
                table: "MovieContent",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieContent_Movies_MovieId",
                table: "MovieContent");

            migrationBuilder.DropTable(
                name: "Collection_Collection");

            migrationBuilder.DropTable(
                name: "Movie_Collection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movies",
                table: "Movies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Collections",
                table: "Collections");

            migrationBuilder.RenameTable(
                name: "Movies",
                newName: "Movie");

            migrationBuilder.RenameTable(
                name: "Collections",
                newName: "MovieCollection");

            migrationBuilder.AddColumn<long>(
                name: "LastTimeStamp",
                table: "MovieContent",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Length",
                table: "MovieContent",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movie",
                table: "Movie",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieCollection",
                table: "MovieCollection",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_MovieContent_Movie_MovieId",
                table: "MovieContent",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
