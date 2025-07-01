using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weather.infra.Migrations
{
    /// <inheritdoc />
    public partial class HumidityWindPrecipitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Humidity",
                table: "WeatherComplete",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Precipitation",
                table: "WeatherComplete",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wind",
                table: "WeatherComplete",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "WeatherComplete");

            migrationBuilder.DropColumn(
                name: "Precipitation",
                table: "WeatherComplete");

            migrationBuilder.DropColumn(
                name: "Wind",
                table: "WeatherComplete");
        }
    }
}
