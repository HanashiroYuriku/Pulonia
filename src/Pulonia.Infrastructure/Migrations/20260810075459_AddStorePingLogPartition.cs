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
                name: "FK_StorePingLogs_Stores_StoreCode",
                table: "StorePingLogs");

            migrationBuilder.Sql(@"
                ALTER TABLE StorePingLogs
                PARTITION BY RANGE (TO_DAYS(CheckedAt)) (
                    PARTITION p_history VALUES LESS THAN (TO_DAYS('2026-08-10')),
                    PARTITION p_20260810 VALUES LESS THAN (TO_DAYS('2026-08-11')),
                    PARTITION p_20260811 VALUES LESS THAN (TO_DAYS('2026-08-12')),
                    PARTITION p_20260812 VALUES LESS THAN (TO_DAYS('2026-08-13')),
                    PARTITION p_20260813 VALUES LESS THAN (TO_DAYS('2026-08-14')),
                    PARTITION p_20260814 VALUES LESS THAN (TO_DAYS('2026-08-15')),
                    PARTITION p_20260815 VALUES LESS THAN (TO_DAYS('2026-08-16')),
                    PARTITION p_20260816 VALUES LESS THAN (TO_DAYS('2026-08-17')),
                    PARTITION p_future VALUES LESS THAN MAXVALUE
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_StorePingLogs_Stores_StoreCode",
                table: "StorePingLogs",
                column: "StoreCode",
                principalTable: "Stores",
                principalColumn: "StoreCode",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
