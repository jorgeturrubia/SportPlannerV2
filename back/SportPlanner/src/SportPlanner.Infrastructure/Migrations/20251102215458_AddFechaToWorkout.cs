using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaToWorkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_workouts_objectives_objective_id",
                table: "workouts");

            migrationBuilder.DropIndex(
                name: "ix_workouts_objective_id",
                table: "workouts");

            migrationBuilder.DropColumn(
                name: "objective_id",
                table: "workouts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "objective_id",
                table: "workouts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_workouts_objective_id",
                table: "workouts",
                column: "objective_id");

            migrationBuilder.AddForeignKey(
                name: "fk_workouts_objectives_objective_id",
                table: "workouts",
                column: "objective_id",
                principalTable: "objectives",
                principalColumn: "id");
        }
    }
}
