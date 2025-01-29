using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMS.Migrations
{
    /// <inheritdoc />
    public partial class AddGitHubLangageDataFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GitHubLanguagesData",
                columns: table => new
                {
                    id_github_langage_data = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom_langage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nombre_repertoire = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHubLanguagesData", x => x.id_github_langage_data);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GitHubLanguagesData");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
