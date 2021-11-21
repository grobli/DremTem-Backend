using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManager.Data.Migrations
{
    public partial class InitialModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Latitude = table.Column<float>(type: "real", nullable: true),
                    Longitude = table.Column<float>(type: "real", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp(0) without time zone", precision: 0, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp(0) without time zone", precision: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => new { x.UserId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "SensorTypes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    DataType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UnitShort = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UnitSymbol = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    IsDiscrete = table.Column<bool>(type: "boolean", nullable: false),
                    IsSummable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorTypes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ApiKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Online = table.Column<bool>(type: "boolean", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp(0) without time zone", precision: 0, nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp(0) without time zone", precision: 0, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp(0) without time zone", precision: 0, nullable: false),
                    LocationName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => new { x.UserId, x.Name });
                    table.ForeignKey(
                        name: "FK_Devices_Locations_UserId_LocationName",
                        columns: x => new { x.UserId, x.LocationName },
                        principalTable: "Locations",
                        principalColumns: new[] { "UserId", "Name" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    TypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => new { x.UserId, x.DeviceName, x.Name });
                    table.ForeignKey(
                        name: "FK_Sensors_Devices_UserId_DeviceName",
                        columns: x => new { x.UserId, x.DeviceName },
                        principalTable: "Devices",
                        principalColumns: new[] { "UserId", "Name" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sensors_SensorTypes_TypeName",
                        column: x => x.TypeName,
                        principalTable: "SensorTypes",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_LocationName",
                table: "Devices",
                column: "LocationName");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_UserId_LocationName",
                table: "Devices",
                columns: new[] { "UserId", "LocationName" });

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_TypeName",
                table: "Sensors",
                column: "TypeName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "SensorTypes");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
