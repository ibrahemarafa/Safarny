using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs_Graduation.Migrations
{
    /// <inheritdoc />
    public partial class yarb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
          
           

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "TouristPlaces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "TouristPlaces",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Range",
                table: "TouristPlaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "Address",
                table: "TouristPlaces");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "TouristPlaces");

            migrationBuilder.DropColumn(
                name: "Range",
                table: "TouristPlaces");

           

            
        }
    }
}
