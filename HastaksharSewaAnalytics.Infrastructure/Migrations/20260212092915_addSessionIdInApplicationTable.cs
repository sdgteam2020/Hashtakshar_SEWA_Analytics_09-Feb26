using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaksharSewaAnalytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addSessionIdInApplicationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActiveSessionId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveSessionIssuedUtc",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveSessionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ActiveSessionIssuedUtc",
                table: "AspNetUsers");
        }
    }
}
