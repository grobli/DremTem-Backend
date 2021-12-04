using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DeviceManager.Data.Migrations
{
    public partial class AddGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    last_modified = table.Column<DateTime>(type: "timestamp(0) without time zone", precision: 0, nullable: true),
                    created = table.Column<DateTime>(type: "timestamp(0) without time zone", precision: 0, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.id);
                    table.UniqueConstraint("ak_groups_name_user_id", x => new { x.name, x.user_id });
                });

            migrationBuilder.CreateTable(
                name: "device_group",
                columns: table => new
                {
                    devices_id = table.Column<int>(type: "integer", nullable: false),
                    groups_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_group", x => new { x.devices_id, x.groups_id });
                    table.ForeignKey(
                        name: "fk_device_group_devices_devices_id",
                        column: x => x.devices_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_device_group_groups_groups_id",
                        column: x => x.groups_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_device_group_groups_id",
                table: "device_group",
                column: "groups_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device_group");

            migrationBuilder.DropTable(
                name: "groups");
        }
    }
}
