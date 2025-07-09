using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Change_Advert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAdverts_UserId_AdvertId",
                table: "UserAdverts");

            migrationBuilder.CreateIndex(
                name: "IX_UserAdverts_UserId",
                table: "UserAdverts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAdverts_UserId",
                table: "UserAdverts");

            migrationBuilder.CreateIndex(
                name: "IX_UserAdverts_UserId_AdvertId",
                table: "UserAdverts",
                columns: new[] { "UserId", "AdvertId" },
                unique: true);
        }
    }
}
