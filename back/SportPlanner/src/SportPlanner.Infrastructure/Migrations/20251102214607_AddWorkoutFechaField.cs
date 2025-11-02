using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutFechaField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_workouts_objectives_objective_id",
                table: "workouts");

            migrationBuilder.DropColumn(
                name: "description",
                table: "workouts");

            migrationBuilder.DropColumn(
                name: "difficulty",
                table: "workouts");

            migrationBuilder.DropColumn(
                name: "name",
                table: "workouts");

            migrationBuilder.RenameIndex(
                name: "IX_Workouts_ObjectiveId",
                table: "workouts",
                newName: "ix_workouts_objective_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha",
                table: "workouts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "fk_workouts_objectives_objective_id",
                table: "workouts",
                column: "objective_id",
                principalTable: "objectives",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_workouts_objectives_objective_id",
                table: "workouts");

            migrationBuilder.DropColumn(
                name: "fecha",
                table: "workouts");

            migrationBuilder.RenameIndex(
                name: "ix_workouts_objective_id",
                table: "workouts",
                newName: "IX_Workouts_ObjectiveId");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "workouts",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "difficulty",
                table: "workouts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "workouts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "fk_workouts_objectives_objective_id",
                table: "workouts",
                column: "objective_id",
                principalTable: "objectives",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
