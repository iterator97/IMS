using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Continentrenamedtoregion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Continent",
                table: "Discounts",
                newName: "Region");

            migrationBuilder.RenameColumn(
                name: "Continent",
                table: "Addresses",
                newName: "Region");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Region",
                table: "Discounts",
                newName: "Continent");

            migrationBuilder.RenameColumn(
                name: "Region",
                table: "Addresses",
                newName: "Continent");
        }
    }
}
