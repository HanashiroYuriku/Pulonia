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
                    PARTITION p_20260818 VALUES LESS THAN (TO_DAYS('2026-08-19')),
                    PARTITION p_20260819 VALUES LESS THAN (TO_DAYS('2026-08-20')),
                    PARTITION p_20260820 VALUES LESS THAN (TO_DAYS('2026-08-21')),
                    PARTITION p_20260821 VALUES LESS THAN (TO_DAYS('2026-08-22')),
                    PARTITION p_20260822 VALUES LESS THAN (TO_DAYS('2026-08-23')),
                    PARTITION p_20260823 VALUES LESS THAN (TO_DAYS('2026-08-24')),
                    PARTITION p_20260824 VALUES LESS THAN (TO_DAYS('2026-08-25')),
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
