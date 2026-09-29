using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaksharSewaAnalytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationAppMasterVersionUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ApplicationVersions_CreatedBy",
                table: "ApplicationVersions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationMasters_CreatedBy",
                table: "ApplicationMasters",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationMasters_AspNetUsers_CreatedBy",
                table: "ApplicationMasters",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationVersions_AspNetUsers_CreatedBy",
                table: "ApplicationVersions",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationMasters_AspNetUsers_CreatedBy",
                table: "ApplicationMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationVersions_AspNetUsers_CreatedBy",
                table: "ApplicationVersions");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationVersions_CreatedBy",
                table: "ApplicationVersions");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationMasters_CreatedBy",
                table: "ApplicationMasters");
        }
    }
}
