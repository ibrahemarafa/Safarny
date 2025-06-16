using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs_Graduation.Migrations
{
    /// <inheritdoc />
    public partial class lastUpReco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "UserInteraction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserInteraction_CityId",
                table: "UserInteraction",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInteraction_Cities_CityId",
                table: "UserInteraction",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInteraction_Cities_CityId",
                table: "UserInteraction");

            migrationBuilder.DropIndex(
                name: "IX_UserInteraction_CityId",
                table: "UserInteraction");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "UserInteraction");
        }
    }
}
