using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinema.Migrations
{
    /// <inheritdoc />
    public partial class AddDateEmailToVouchers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Vouchers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpirationDate",
                table: "Vouchers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Vouchers");
        }
    }
}
