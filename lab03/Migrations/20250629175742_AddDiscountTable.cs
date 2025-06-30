using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lab03.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Discounts",
                newName: "Value");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "Discounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Discounts",
                newName: "Amount");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "Discounts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
