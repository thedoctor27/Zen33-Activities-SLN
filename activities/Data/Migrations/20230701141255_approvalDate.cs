using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace activities.Data.Migrations
{
    /// <inheritdoc />
    public partial class approvalDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "UserProfiles");
        }
    }
}
