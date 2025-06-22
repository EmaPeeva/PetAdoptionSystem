using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAdoptionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAdopterIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdopterId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AdopterId",
                table: "AspNetUsers",
                column: "AdopterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Adopters_AdopterId",
                table: "AspNetUsers",
                column: "AdopterId",
                principalTable: "Adopters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Adopters_AdopterId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AdopterId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AdopterId",
                table: "AspNetUsers");
        }
    }
}
