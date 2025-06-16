using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs_Graduation.Migrations
{
    /// <inheritdoc />
    public partial class updates_reco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "UserInteraction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TouristPlaceId",
                table: "UserInteraction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "UserInteraction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Popularity",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserInteraction_ActivityId",
                table: "UserInteraction",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteraction_TouristPlaceId",
                table: "UserInteraction",
                column: "TouristPlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInteraction_Activities_ActivityId",
                table: "UserInteraction",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInteraction_TouristPlaces_TouristPlaceId",
                table: "UserInteraction",
                column: "TouristPlaceId",
                principalTable: "TouristPlaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInteraction_Activities_ActivityId",
                table: "UserInteraction");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInteraction_TouristPlaces_TouristPlaceId",
                table: "UserInteraction");

            migrationBuilder.DropIndex(
                name: "IX_UserInteraction_ActivityId",
                table: "UserInteraction");

            migrationBuilder.DropIndex(
                name: "IX_UserInteraction_TouristPlaceId",
                table: "UserInteraction");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "UserInteraction");

            migrationBuilder.DropColumn(
                name: "TouristPlaceId",
                table: "UserInteraction");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "UserInteraction");

            migrationBuilder.DropColumn(
                name: "Popularity",
                table: "Activities");
        }
    }
}
