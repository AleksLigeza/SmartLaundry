using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartLaundry.Data.Migrations
{
    public partial class NullableIdReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WashingMachines_Laundries_LaundryId",
                table: "WashingMachines");

            migrationBuilder.AlterColumn<int>(
                name: "LaundryId",
                table: "WashingMachines",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_WashingMachines_Laundries_LaundryId",
                table: "WashingMachines",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WashingMachines_Laundries_LaundryId",
                table: "WashingMachines");

            migrationBuilder.AlterColumn<int>(
                name: "LaundryId",
                table: "WashingMachines",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WashingMachines_Laundries_LaundryId",
                table: "WashingMachines",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
