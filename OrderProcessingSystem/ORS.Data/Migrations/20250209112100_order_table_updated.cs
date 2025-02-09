using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ORS.Data.Migrations
{
    /// <inheritdoc />
    public partial class order_table_updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "IsFulfilled",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId_IsFulfilled",
                table: "Orders",
                columns: new[] { "CustomerId", "IsFulfilled" },
                unique: true,
                filter: "[IsFulfilled] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId_IsFulfilled",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsFulfilled",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");
        }
    }
}
