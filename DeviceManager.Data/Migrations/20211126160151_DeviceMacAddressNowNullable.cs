using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManager.Data.Migrations
{
    public partial class DeviceMacAddressNowNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "ak_devices_mac_address",
                table: "devices");

            migrationBuilder.AlterColumn<string>(
                name: "mac_address",
                table: "devices",
                type: "character varying(17)",
                maxLength: 17,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(17)",
                oldMaxLength: 17);

            migrationBuilder.CreateIndex(
                name: "ix_devices_mac_address",
                table: "devices",
                column: "mac_address",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_devices_mac_address",
                table: "devices");

            migrationBuilder.AlterColumn<string>(
                name: "mac_address",
                table: "devices",
                type: "character varying(17)",
                maxLength: 17,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(17)",
                oldMaxLength: 17,
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_devices_mac_address",
                table: "devices",
                column: "mac_address");
        }
    }
}
