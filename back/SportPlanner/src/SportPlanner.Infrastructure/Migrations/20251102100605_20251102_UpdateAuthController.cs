using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20251102_UpdateAuthController : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Exercises_CategoryId",
                table: "exercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_TypeId",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "default_duration_seconds",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "default_intensity",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "default_reps",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "default_sets",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "type_id",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "video_url",
                table: "exercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                table: "exercises",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "default_duration_seconds",
                table: "exercises",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "default_intensity",
                table: "exercises",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "default_reps",
                table: "exercises",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "default_sets",
                table: "exercises",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "exercises",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "type_id",
                table: "exercises",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "video_url",
                table: "exercises",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_CategoryId",
                table: "exercises",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_TypeId",
                table: "exercises",
                column: "type_id");
        }
    }
}
