using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHomeVenueFromTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "home_venue",
                table: "teams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "home_venue",
                table: "teams",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
