using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMS.Migrations
{
    /// <inheritdoc />
    public partial class AddGitHubLangageDataFieldV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GitHubLanguagesData",
                table: "GitHubLanguagesData");

            migrationBuilder.RenameTable(
                name: "GitHubLanguagesData",
                newName: "GitHubLangagesData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GitHubLangagesData",
                table: "GitHubLangagesData",
                column: "id_github_langage_data");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GitHubLangagesData",
                table: "GitHubLangagesData");

            migrationBuilder.RenameTable(
                name: "GitHubLangagesData",
                newName: "GitHubLanguagesData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GitHubLanguagesData",
                table: "GitHubLanguagesData",
                column: "id_github_langage_data");
        }
    }
}
