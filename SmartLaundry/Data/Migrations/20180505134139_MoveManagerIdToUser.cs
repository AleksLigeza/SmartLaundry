using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartLaundry.Data.Migrations
{
    public partial class MoveManagerIdToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dormitories_AspNetUsers_ManagerId",
                table: "Dormitories");

            migrationBuilder.DropIndex(
                name: "IX_Dormitories_ManagerId",
                table: "Dormitories");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Dormitories");

            migrationBuilder.AddColumn<int>(
                name: "DormitoryManagerId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DormitoryManagerId",
                table: "AspNetUsers",
                column: "DormitoryManagerId",
                unique: true,
                filter: "[DormitoryManagerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryManagerId",
                table: "AspNetUsers",
                column: "DormitoryManagerId",
                principalTable: "Dormitories",
                principalColumn: "DormitoryID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DormitoryManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DormitoryManagerId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "Dormitories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dormitories_ManagerId",
                table: "Dormitories",
                column: "ManagerId",
                unique: true,
                filter: "[ManagerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Dormitories_AspNetUsers_ManagerId",
                table: "Dormitories",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
