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
            
            // timescaledb - initial setup
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS timescaledb");
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS timescaledb_toolkit");
            migrationBuilder.Sql("ALTER EXTENSION timescaledb_toolkit UPDATE");
            
            // create readings hyper-table
            migrationBuilder.Sql("SELECT create_hypertable('readings', 'time', 'sensor_id', 4)");
            
            // create continuous aggregates - daily metrics
            migrationBuilder.Sql(@"
               CREATE MATERIALIZED VIEW reading_metrics_daily
                WITH (timescaledb.continuous)
                AS
                SELECT 
	                time_bucket('1 day'::interval, time) as bucket, 
	                sensor_id,
	                stats_agg(value) as stats,
	                max(value) as max_value,
	                min(value) as min_value
                FROM 
	                readings
                GROUP BY bucket, sensor_id
                WITH NO DATA;
            ");
            
            // daily metrics - create refresh policy - refresh last 6 months' data every 2 weeks
            migrationBuilder.Sql(@"
				SELECT add_continuous_aggregate_policy('reading_metrics_daily',
					start_offset => INTERVAL '6 month',
					end_offset => INTERVAL '1 h',
					schedule_interval => INTERVAL '14 day');
			");
            
            // create continuous aggregates - hourly metrics
            migrationBuilder.Sql(@"
               CREATE MATERIALIZED VIEW reading_metrics_hourly
                WITH (timescaledb.continuous)
                AS
                SELECT 
	                time_bucket('1 hour'::interval, time) as bucket, 
	                sensor_id,
	                stats_agg(value) as stats,
	                max(value) as max_value,
	                min(value) as min_value
                FROM 
	                readings
                GROUP BY bucket, sensor_id
                WITH NO DATA;
            ");
            
            // hourly metrics - create refresh policy - refresh last month's data every hour
            migrationBuilder.Sql(@"
				SELECT add_continuous_aggregate_policy('reading_metrics_hourly',
					start_offset => INTERVAL '1 month',
					end_offset => INTERVAL '1 h',
					schedule_interval => INTERVAL '1 h');
			");
            
            // reading daily metrics summary
            migrationBuilder.Sql(@"
                CREATE VIEW reading_metrics_daily_summary
				AS
				SELECT
					bucket,
					sensor_id,
					max_value, 
					min_value, average(stats) as avg_value,
					sum(stats) as sum_value,
					stddev(stats),
					variance(stats),
					num_vals(stats)
				FROM 
					reading_metrics_daily;
            ");
            
            // reading daily metrics summary
            migrationBuilder.Sql(@"
                CREATE VIEW reading_metrics_hourly_summary
				AS
				SELECT
					bucket,
					sensor_id,
					max_value, 
					min_value, average(stats) as avg_value,
					sum(stats) as sum_value,
					stddev(stats),
					variance(stats),
					num_vals(stats)
				FROM 
					reading_metrics_hourly;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "readings");

            migrationBuilder.Sql("DROP VIEW reading_metrics_hourly_summary");
            migrationBuilder.Sql("DROP VIEW reading_metrics_daily_summary");
            migrationBuilder.Sql("DROP MATERIALIZED VIEW reading_metrics_hourly");
            migrationBuilder.Sql("DROP MATERIALIZED VIEW reading_metrics_daily");
        }
    }
}
