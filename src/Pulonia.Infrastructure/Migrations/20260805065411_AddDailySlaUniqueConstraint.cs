using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulonia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailySlaUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropIndex(
            //     name: "IX_DailySlas_StoreCode",
            //     table: "DailySlas");

            migrationBuilder.CreateIndex(
                name: "IX_DailySlas_StoreCode_Date",
                table: "DailySlas",
                columns: new[] { "StoreCode", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailySlas_StoreCode_Date",
                table: "DailySlas");

            // migrationBuilder.CreateIndex(
            //     name: "IX_DailySlas_StoreCode",
            //     table: "DailySlas",
            //     column: "StoreCode");
        }
    }
}
