using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManager.Data.Migrations
{
    public partial class AddIndexToSensor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_sensors_name_device_id",
                table: "sensors",
                columns: new[] { "name", "device_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sensors_name_device_id",
                table: "sensors");
        }
    }
}
