using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs_Graduation.Migrations
{
    /// <inheritdoc />
    public partial class carttrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Carts");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "TouristPlaces",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "StartPrice",
                table: "Hotels",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Cities",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CityId",
                table: "Carts",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Cities_CityId",
                table: "Carts",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Cities_CityId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_CityId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Carts");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "TouristPlaces",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "StartPrice",
                table: "Hotels",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
