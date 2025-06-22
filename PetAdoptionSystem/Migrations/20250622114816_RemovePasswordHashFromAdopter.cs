using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAdoptionSystem.Migrations
{
    /// <inheritdoc />
    public partial class RemovePasswordHashFromAdopter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Adopters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Adopters",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
