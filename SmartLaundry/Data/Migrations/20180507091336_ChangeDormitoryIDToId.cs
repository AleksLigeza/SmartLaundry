using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartLaundry.Data.Migrations
{
    public partial class ChangeDormitoryIDToId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DormitoryID",
                table: "Dormitories",
                newName: "DormitoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DormitoryId",
                table: "Dormitories",
                newName: "DormitoryID");
        }
    }
}
