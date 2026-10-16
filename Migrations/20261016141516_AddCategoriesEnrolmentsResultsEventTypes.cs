using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_raceday_part2.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesEnrolmentsResultsEventTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumAge = table.Column<int>(type: "int", nullable: false),
                    MaximumAge = table.Column<int>(type: "int", nullable: true),
                    Distance = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
                    table.ForeignKey(
                        name: "FK_Categories_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    EventTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.EventTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Enrolments",
                columns: table => new
                {
                    EnrolmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrolments", x => x.EnrolmentID);
                    table.ForeignKey(
                        name: "FK_Enrolments_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "FK_Enrolments_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "EventID");
                    table.ForeignKey(
                        name: "FK_Enrolments_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    ResultID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrolmentID = table.Column<int>(type: "int", nullable: false),
                    FinishTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    FinishPosition = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.ResultID);
                    table.ForeignKey(
                        name: "FK_Results_Enrolments_EnrolmentID",
                        column: x => x.EnrolmentID,
                        principalTable: "Enrolments",
                        principalColumn: "EnrolmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "EventTypeID", "TypeName" },
                values: new object[,]
                {
                    { 1, "Run" },
                    { 2, "Walk" },
                    { 3, "Cycle" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTypeID",
                table: "Events",
                column: "EventTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_EventID",
                table: "Categories",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_CategoryID",
                table: "Enrolments",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_EventID",
                table: "Enrolments",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_UserID",
                table: "Enrolments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Results_EnrolmentID",
                table: "Results",
                column: "EnrolmentID",
                unique: true);

            migrationBuilder.Sql(
                "UPDATE dbo.Events SET EventTypeID = 1 WHERE EventTypeID = 0;");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventTypes_EventTypeID",
                table: "Events",
                column: "EventTypeID",
                principalTable: "EventTypes",
                principalColumn: "EventTypeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventTypes_EventTypeID",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Enrolments");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventTypeID",
                table: "Events");
        }
    }
}
