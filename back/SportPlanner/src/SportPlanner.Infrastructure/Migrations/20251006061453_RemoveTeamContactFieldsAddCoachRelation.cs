using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTeamContactFieldsAddCoachRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "coach_name",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "contact_email",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "contact_phone",
                table: "teams");

            migrationBuilder.AddColumn<Guid>(
                name: "coach_subscription_user_id",
                table: "teams",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_teams_coach_subscription_user_id",
                table: "teams",
                column: "coach_subscription_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_teams_subscription_users_coach_subscription_user_id",
                table: "teams",
                column: "coach_subscription_user_id",
                principalTable: "subscription_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_teams_subscription_users_coach_subscription_user_id",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "ix_teams_coach_subscription_user_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "coach_subscription_user_id",
                table: "teams");

            migrationBuilder.AddColumn<string>(
                name: "coach_name",
                table: "teams",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_email",
                table: "teams",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_phone",
                table: "teams",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
