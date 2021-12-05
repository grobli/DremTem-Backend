using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SensorData.Data.Migrations
{
    public partial class InitialModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "readings",
                columns: table => new
                {
                    time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    sensor_id = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_readings", x => new { x.time, x.sensor_id });
                });
            
            // timescaledb stuff
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS timescaledb");
            migrationBuilder.Sql("SELECT create_hypertable('readings', 'time')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "readings");
        }
    }
}
