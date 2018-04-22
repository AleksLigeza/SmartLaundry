using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmartLaundry.Data.Migrations
{
    public partial class Dormitorymanagerfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryManagerID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryPorterID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DormitoryManagerID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DormitoryManagerID",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "DormitoryPorterID",
                table: "AspNetUsers",
                newName: "DormitoryPorterId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_DormitoryPorterID",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_DormitoryPorterId");

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
                name: "FK_AspNetUsers_Dormitories_DormitoryPorterId",
                table: "AspNetUsers",
                column: "DormitoryPorterId",
                principalTable: "Dormitories",
                principalColumn: "DormitoryID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dormitories_AspNetUsers_ManagerId",
                table: "Dormitories",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryPorterId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Dormitories_AspNetUsers_ManagerId",
                table: "Dormitories");

            migrationBuilder.DropIndex(
                name: "IX_Dormitories_ManagerId",
                table: "Dormitories");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Dormitories");

            migrationBuilder.RenameColumn(
                name: "DormitoryPorterId",
                table: "AspNetUsers",
                newName: "DormitoryPorterID");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_DormitoryPorterId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_DormitoryPorterID");

            migrationBuilder.AddColumn<int>(
                name: "DormitoryManagerID",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DormitoryManagerID",
                table: "AspNetUsers",
                column: "DormitoryManagerID",
                unique: true,
                filter: "[DormitoryManagerID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryManagerID",
                table: "AspNetUsers",
                column: "DormitoryManagerID",
                principalTable: "Dormitories",
                principalColumn: "DormitoryID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Dormitories_DormitoryPorterID",
                table: "AspNetUsers",
                column: "DormitoryPorterID",
                principalTable: "Dormitories",
                principalColumn: "DormitoryID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
