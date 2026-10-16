using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_raceday_part2.Migrations
{
    /// <inheritdoc />
    public partial class AddEventOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // 1. Add the column temporarily as nullable.
            migrationBuilder.AddColumn<int>(
                name: "OrganizerID",
                table: "Events",
                type: "int",
                nullable: true);

            // 2. Assign the existing events to Victor.
            migrationBuilder.Sql(
                "UPDATE [Events] SET [OrganizerID] = 1;");

            // 3. Make the column required after backfilling.
            migrationBuilder.AlterColumn<int>(
                name: "OrganizerID",
                table: "Events",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 4. Create the index.
            migrationBuilder.CreateIndex(
                name: "IX_Events_OrganizerID",
                table: "Events",
                column: "OrganizerID");

            // 5. Add the foreign key.
            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_OrganizerID",
                table: "Events",
                column: "OrganizerID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_OrganizerID",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_OrganizerID",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OrganizerID",
                table: "Events");
        }
    }
}
