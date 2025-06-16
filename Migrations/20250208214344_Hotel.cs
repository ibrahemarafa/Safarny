using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs_Graduation.Migrations
{
    /// <inheritdoc />
    public partial class Hotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Hotel_Cities_CityId",
            //    table: "Hotel");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Trips_Hotel_HotelId",
            //    table: "Trips");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_Hotel",
            //    table: "Hotel");

            //migrationBuilder.RenameTable(
            //    name: "Hotel",
            //    newName: "Hotels");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Hotel_CityId",
            //    table: "Hotels",
            //    newName: "IX_Hotels_CityId");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_Hotels",
            //    table: "Hotels",
            //    column: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Hotels_Cities_CityId",
            //    table: "Hotels",
            //    column: "CityId",
            //    principalTable: "Cities",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Trips_Hotels_HotelId",
            //    table: "Trips",
            //    column: "HotelId",
            //    principalTable: "Hotels",
            //    principalColumn: "Id");


            // Drop existing foreign keys
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Hotel_Cities_CityId",
            //    table: "Hotels");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Trips_Hotel_HotelId",
            //    table: "Trips");

            // Drop the primary key
            // Add the CityId column
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Hotels",
                nullable: false,
                defaultValue: 0); // Adjust default value as needed

            // Add the foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Cities_CityId",
                table: "Hotels",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

      

    

       

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Hotels_Cities_CityId",
            //    table: "Hotels");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Trips_Hotels_HotelId",
            //    table: "Trips");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_Hotels",
            //    table: "Hotels");

            //migrationBuilder.RenameTable(
            //    name: "Hotels",
            //    newName: "Hotel");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Hotels_CityId",
            //    table: "Hotel",
            //    newName: "IX_Hotel_CityId");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_Hotel",
            //    table: "Hotel",
            //    column: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Hotel_Cities_CityId",
            //    table: "Hotel",
            //    column: "CityId",
            //    principalTable: "Cities",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Trips_Hotel_HotelId",
            //    table: "Trips",
            //    column: "HotelId",
            //    principalTable: "Hotel",
            //    principalColumn: "Id");


       
        
            // Drop foreign keys...

            // Drop the primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_Hotels",
                table: "Hotels");

            // Rename the table back with schema
            migrationBuilder.RenameTable(
                name: "Hotels",
                schema: "dbo", // Add schema
                newName: "Hotel");

            // Rename the index back
            migrationBuilder.RenameIndex(
                name: "IX_Hotels_CityId",
                table: "Hotel",
                newName: "IX_Hotel_CityId");

            // Add the primary key back...
        }

    }
    }
