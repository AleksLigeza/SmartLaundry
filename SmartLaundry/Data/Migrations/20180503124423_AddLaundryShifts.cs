using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartLaundry.Data.Migrations
{
    public partial class AddLaundryShifts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "shiftCount",
                table: "Laundries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "shiftTime",
                table: "Laundries",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "startTime",
                table: "Laundries",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shiftCount",
                table: "Laundries");

            migrationBuilder.DropColumn(
                name: "shiftTime",
                table: "Laundries");

            migrationBuilder.DropColumn(
                name: "startTime",
                table: "Laundries");
        }
    }
}
