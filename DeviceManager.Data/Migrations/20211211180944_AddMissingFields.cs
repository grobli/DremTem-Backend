using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManager.Data.Migrations
{
    public partial class AddMissingFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "display_name",
                table: "sensor_types",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "display_name",
                table: "sensor_types");
        }
    }
}
