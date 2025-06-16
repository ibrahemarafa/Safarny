using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs_Graduation.Migrations
{
    /// <inheritdoc />
    public partial class BookingPaymentTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomTripBookingId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookingCustomTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCustomTrips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingCustomTripDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCustomTripDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingCustomTripDetails_BookingCustomTrips_BookingId",
                        column: x => x.BookingId,
                        principalTable: "BookingCustomTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingCustomTripDetails_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingCustomTripDetails_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomTripBookingId",
                table: "Payments",
                column: "CustomTripBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCustomTripDetails_BookingId",
                table: "BookingCustomTripDetails",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCustomTripDetails_CityId",
                table: "BookingCustomTripDetails",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCustomTripDetails_HotelId",
                table: "BookingCustomTripDetails",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_BookingCustomTrips_CustomTripBookingId",
                table: "Payments",
                column: "CustomTripBookingId",
                principalTable: "BookingCustomTrips",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_BookingCustomTrips_CustomTripBookingId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "BookingCustomTripDetails");

            migrationBuilder.DropTable(
                name: "BookingCustomTrips");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CustomTripBookingId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CustomTripBookingId",
                table: "Payments");
        }
    }
}
