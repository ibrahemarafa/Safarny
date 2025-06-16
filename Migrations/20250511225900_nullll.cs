using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs_Graduation.Migrations
{
    /// <inheritdoc />
    public partial class nullll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_Features_Hotels_HotelId",
                table: "Hotel_Features");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_Images_Hotels_HotelId",
                table: "Hotel_Images");

            migrationBuilder.DropForeignKey(
                name: "FK_HotelBookings_Hotels_HotelId",
                table: "HotelBookings");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "HotelBookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Hotel_Images",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Hotel_Features",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "BookingCustomTripDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_Features_Hotels_HotelId",
                table: "Hotel_Features",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_Images_Hotels_HotelId",
                table: "Hotel_Images",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelBookings_Hotels_HotelId",
                table: "HotelBookings",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_Features_Hotels_HotelId",
                table: "Hotel_Features");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotel_Images_Hotels_HotelId",
                table: "Hotel_Images");

            migrationBuilder.DropForeignKey(
                name: "FK_HotelBookings_Hotels_HotelId",
                table: "HotelBookings");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "HotelBookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Hotel_Images",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Hotel_Features",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "BookingCustomTripDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_Features_Hotels_HotelId",
                table: "Hotel_Features",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotel_Images_Hotels_HotelId",
                table: "Hotel_Images",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HotelBookings_Hotels_HotelId",
                table: "HotelBookings",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
