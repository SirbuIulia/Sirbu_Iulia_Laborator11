using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sirbu_Iulia_Lab2.Migrations
{
    /// <inheritdoc />
    public partial class City : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityID",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CityID",
                table: "Customer",
                column: "CityID");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_City_CityID",
                table: "Customer",
                column: "CityID",
                principalTable: "City",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_City_CityID",
                table: "Customer");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropIndex(
                name: "IX_Customer_CityID",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CityID",
                table: "Customer");
        }
    }
}
