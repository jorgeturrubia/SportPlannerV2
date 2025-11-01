using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20251101_UpdateObjectivesPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_exercises_exercise_categories_category_id",
                table: "exercises");

            migrationBuilder.DropForeignKey(
                name: "fk_exercises_exercise_types_type_id",
                table: "exercises");

            migrationBuilder.DropTable(
                name: "exercise_categories");

            migrationBuilder.DropTable(
                name: "exercise_types");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercise_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sport = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercise_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exercise_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    requires_duration = table.Column<bool>(type: "boolean", nullable: false),
                    requires_reps = table.Column<bool>(type: "boolean", nullable: false),
                    requires_sets = table.Column<bool>(type: "boolean", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercise_types", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseCategories_Sport_IsActive",
                table: "exercise_categories",
                columns: new[] { "sport", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTypes_IsActive",
                table: "exercise_types",
                column: "is_active");

            migrationBuilder.AddForeignKey(
                name: "fk_exercises_exercise_categories_category_id",
                table: "exercises",
                column: "category_id",
                principalTable: "exercise_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_exercises_exercise_types_type_id",
                table: "exercises",
                column: "type_id",
                principalTable: "exercise_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
