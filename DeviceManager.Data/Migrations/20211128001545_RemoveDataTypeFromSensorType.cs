using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManager.Data.Migrations
{
    public partial class RemoveDataTypeFromSensorType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_type",
                table: "sensor_types");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "data_type",
                table: "sensor_types",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
