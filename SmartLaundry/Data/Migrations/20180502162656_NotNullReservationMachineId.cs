using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartLaundry.Data.Migrations
{
    public partial class NotNullReservationMachineId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_WashingMachines_WashingMachineId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_WashingMachines_Laundries_LaundryId",
                table: "WashingMachines");

            migrationBuilder.AlterColumn<int>(
                name: "LaundryId",
                table: "WashingMachines",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "WashingMachines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "WashingMachineId",
                table: "Reservations",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_WashingMachines_WashingMachineId",
                table: "Reservations",
                column: "WashingMachineId",
                principalTable: "WashingMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WashingMachines_Laundries_LaundryId",
                table: "WashingMachines",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_WashingMachines_WashingMachineId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_WashingMachines_Laundries_LaundryId",
                table: "WashingMachines");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "WashingMachines");

            migrationBuilder.AlterColumn<int>(
                name: "LaundryId",
                table: "WashingMachines",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "WashingMachineId",
                table: "Reservations",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_WashingMachines_WashingMachineId",
                table: "Reservations",
                column: "WashingMachineId",
                principalTable: "WashingMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WashingMachines_Laundries_LaundryId",
                table: "WashingMachines",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
