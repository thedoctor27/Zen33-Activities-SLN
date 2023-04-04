using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace activities.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingOtherField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Other",
                table: "UserProfiles",
                type: "nvarchar(65)",
                maxLength: 65,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Other",
                table: "UserProfiles");
        }
    }
}
