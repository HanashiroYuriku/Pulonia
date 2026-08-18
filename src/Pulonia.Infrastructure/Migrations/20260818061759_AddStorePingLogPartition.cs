using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulonia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStorePingLogPartition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StorePingLogs_Stores_StoreId",
                table: "StorePingLogs");

            migrationBuilder.Sql(@"
                ALTER TABLE StorePingLogs
                PARTITION BY RANGE (TO_DAYS(CheckedAt)) (
                    PARTITION p_history VALUES LESS THAN (TO_DAYS('2026-08-18')),
                    PARTITION p_20260810 VALUES LESS THAN (TO_DAYS('2026-08-19')),
                    PARTITION p_20260811 VALUES LESS THAN (TO_DAYS('2026-08-20')),
                    PARTITION p_20260812 VALUES LESS THAN (TO_DAYS('2026-08-21')),
                    PARTITION p_20260813 VALUES LESS THAN (TO_DAYS('2026-08-22')),
                    PARTITION p_20260814 VALUES LESS THAN (TO_DAYS('2026-08-23')),
                    PARTITION p_20260815 VALUES LESS THAN (TO_DAYS('2026-08-24')),
                    PARTITION p_20260816 VALUES LESS THAN (TO_DAYS('2026-08-25')),
                    PARTITION p_future VALUES LESS THAN MAXVALUE
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
