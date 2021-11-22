using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManager.Data.Migrations
{
    public partial class FixSensorForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_Devices_DeviceId",
                table: "Sensors");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_SensorTypes_TypeName",
                table: "Sensors");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_Devices_DeviceId",
                table: "Sensors",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_SensorTypes_TypeName",
                table: "Sensors",
                column: "TypeName",
                principalTable: "SensorTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_Devices_DeviceId",
                table: "Sensors");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_SensorTypes_TypeName",
                table: "Sensors");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_Devices_DeviceId",
                table: "Sensors",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_SensorTypes_TypeName",
                table: "Sensors",
                column: "TypeName",
                principalTable: "SensorTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
