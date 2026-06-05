using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DiscountentityaddedAddressextendedwithContinentOrderextebdedwithLocationCharge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LocationCharge",
                table: "Orders",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "OrderItems",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Continent",
                table: "Addresses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Continent = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Mode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_LocationCharge_Range",
                table: "Orders",
                sql: "\"LocationCharge\" >= 0 AND \"LocationCharge\" <= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_TotalAmount_NonNegative",
                table: "Orders",
                sql: "\"TotalAmount\" >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_LocationCharge_Range",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_TotalAmount_NonNegative",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LocationCharge",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Continent",
                table: "Addresses");
        }
    }
}
