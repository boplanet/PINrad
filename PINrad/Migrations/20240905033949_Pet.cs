using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PINrad.Migrations
{
    /// <inheritdoc />
    public partial class Pet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_AspNetUsers_RegLogUserId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Assets_AssetId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_RegLogUserId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "RegLogUserId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "CustomUserId",
                table: "Assignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AssetId",
                table: "Assignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CustomUserId",
                table: "Assignments",
                column: "CustomUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Assets_AssetId",
                table: "Assignments",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_CustomUsers_CustomUserId",
                table: "Assignments",
                column: "CustomUserId",
                principalTable: "CustomUsers",
                principalColumn: "CustomUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Assets_AssetId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_CustomUsers_CustomUserId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_CustomUserId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "CustomUserId",
                table: "Assignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssetId",
                table: "Assignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegLogUserId",
                table: "Assignments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_RegLogUserId",
                table: "Assignments",
                column: "RegLogUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_AspNetUsers_RegLogUserId",
                table: "Assignments",
                column: "RegLogUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Assets_AssetId",
                table: "Assignments",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
