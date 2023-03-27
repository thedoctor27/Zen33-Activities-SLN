using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace activities.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingAvailableField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "UserProfiles");
        }
    }
}
