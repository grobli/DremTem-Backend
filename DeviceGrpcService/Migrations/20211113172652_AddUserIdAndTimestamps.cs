using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceGrpcService.Migrations
{
    public partial class AddUserIdAndTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_Location_LocationID",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_Device_DeviceID",
                table: "Sensor");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_SensorType_TypeID",
                table: "Sensor");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "SensorType",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TypeID",
                table: "Sensor",
                newName: "TypeId");

            migrationBuilder.RenameColumn(
                name: "DeviceID",
                table: "Sensor",
                newName: "DeviceId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Sensor",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Sensor_TypeID",
                table: "Sensor",
                newName: "IX_Sensor_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Sensor_DeviceID",
                table: "Sensor",
                newName: "IX_Sensor_DeviceId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Location",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "LocationID",
                table: "Device",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Device",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Device_LocationID",
                table: "Device",
                newName: "IX_Device_LocationId");

            migrationBuilder.AlterColumn<string>(
                name: "UnitShort",
                table: "SensorType",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "SensorType",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SensorType",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataType",
                table: "SensorType",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Location",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Location",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Location",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "Device",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Device",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Device",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeen",
                table: "Device",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Device",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Location_LocationId",
                table: "Device",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_Device_DeviceId",
                table: "Sensor",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_SensorType_TypeId",
                table: "Sensor",
                column: "TypeId",
                principalTable: "SensorType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_Location_LocationId",
                table: "Device");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_Device_DeviceId",
                table: "Sensor");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_SensorType_TypeId",
                table: "Sensor");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "LastSeen",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Device");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SensorType",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Sensor",
                newName: "TypeID");

            migrationBuilder.RenameColumn(
                name: "DeviceId",
                table: "Sensor",
                newName: "DeviceID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Sensor",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_Sensor_TypeId",
                table: "Sensor",
                newName: "IX_Sensor_TypeID");

            migrationBuilder.RenameIndex(
                name: "IX_Sensor_DeviceId",
                table: "Sensor",
                newName: "IX_Sensor_DeviceID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Location",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Device",
                newName: "LocationID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Device",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_Device_LocationId",
                table: "Device",
                newName: "IX_Device_LocationID");

            migrationBuilder.AlterColumn<string>(
                name: "UnitShort",
                table: "SensorType",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "SensorType",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SensorType",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DataType",
                table: "SensorType",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Location_LocationID",
                table: "Device",
                column: "LocationID",
                principalTable: "Location",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_Device_DeviceID",
                table: "Sensor",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_SensorType_TypeID",
                table: "Sensor",
                column: "TypeID",
                principalTable: "SensorType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
