using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitmeal_workout.Migrations
{
    /// <inheritdoc />
    public partial class addPaymentStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "paymentStatus",
                table: "ordersDetails",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paymentStatus",
                table: "ordersDetails");
        }
    }
}
