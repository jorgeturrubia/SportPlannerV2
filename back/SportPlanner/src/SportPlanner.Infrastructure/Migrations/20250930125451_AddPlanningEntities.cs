using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanningEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercise_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Sport = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "exercise_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RequiresSets = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresReps = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresDuration = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "marketplace_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Sport = table.Column<string>(type: "text", nullable: false),
                    SourceEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceOwnership = table.Column<string>(type: "text", nullable: false),
                    PublishedBySubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsSystemOfficial = table.Column<bool>(type: "boolean", nullable: false),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    TotalRatings = table.Column<int>(type: "integer", nullable: false),
                    TotalDownloads = table.Column<int>(type: "integer", nullable: false),
                    TotalViews = table.Column<int>(type: "integer", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marketplace_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_marketplace_items_Subscriptions_PublishedBySubscriptionId",
                        column: x => x.PublishedBySubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Ownership = table.Column<string>(type: "text", nullable: false),
                    MarketplaceUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ObjectiveId = table.Column<Guid>(type: "uuid", nullable: true),
                    EstimatedDurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    Difficulty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workouts_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_workouts_objectives_ObjectiveId",
                        column: x => x.ObjectiveId,
                        principalTable: "objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Ownership = table.Column<string>(type: "text", nullable: false),
                    MarketplaceUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    VideoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Instructions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DefaultSets = table.Column<int>(type: "integer", nullable: true),
                    DefaultReps = table.Column<int>(type: "integer", nullable: true),
                    DefaultDurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    DefaultIntensity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exercises_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exercises_exercise_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "exercise_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_exercises_exercise_types_TypeId",
                        column: x => x.TypeId,
                        principalTable: "exercise_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "marketplace_ratings",
                columns: table => new
                {
                    MarketplaceItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    RatedBySubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Stars = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marketplace_ratings", x => new { x.MarketplaceItemId, x.RatedBySubscriptionId });
                    table.ForeignKey(
                        name: "FK_marketplace_ratings_Subscriptions_RatedBySubscriptionId",
                        column: x => x.RatedBySubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_marketplace_ratings_marketplace_items_MarketplaceItemId",
                        column: x => x.MarketplaceItemId,
                        principalTable: "marketplace_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_objectives",
                columns: table => new
                {
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectiveId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_objectives", x => new { x.WorkoutId, x.ObjectiveId });
                    table.ForeignKey(
                        name: "FK_workout_objectives_objectives_ObjectiveId",
                        column: x => x.ObjectiveId,
                        principalTable: "objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_workout_objectives_workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exercise_objectives",
                columns: table => new
                {
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectiveId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_objectives", x => new { x.ExerciseId, x.ObjectiveId });
                    table.ForeignKey(
                        name: "FK_exercise_objectives_exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exercise_objectives_objectives_ObjectiveId",
                        column: x => x.ObjectiveId,
                        principalTable: "objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_exercises",
                columns: table => new
                {
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Sets = table.Column<int>(type: "integer", nullable: true),
                    Reps = table.Column<int>(type: "integer", nullable: true),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    Intensity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RestSeconds = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_exercises", x => new { x.WorkoutId, x.ExerciseId });
                    table.ForeignKey(
                        name: "FK_workout_exercises_exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_workout_exercises_workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseCategories_Sport_IsActive",
                table: "exercise_categories",
                columns: new[] { "Sport", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseObjectives_ObjectiveId",
                table: "exercise_objectives",
                column: "ObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTypes_IsActive",
                table: "exercise_types",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_CategoryId",
                table: "exercises",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_Ownership_IsActive",
                table: "exercises",
                columns: new[] { "Ownership", "IsActive" },
                filter: "subscription_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_SubscriptionId_IsActive",
                table: "exercises",
                columns: new[] { "SubscriptionId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_TypeId",
                table: "exercises",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_PublishedBySubscriptionId",
                table: "marketplace_items",
                column: "PublishedBySubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_SourceEntityId",
                table: "marketplace_items",
                column: "SourceEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_Sport_Downloads",
                table: "marketplace_items",
                columns: new[] { "Sport", "TotalDownloads" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_Sport_Official_Rating",
                table: "marketplace_items",
                columns: new[] { "Sport", "IsSystemOfficial", "AverageRating" },
                descending: new[] { false, false, true },
                filter: "is_system_official = true");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_Sport_Type_Rating",
                table: "marketplace_items",
                columns: new[] { "Sport", "Type", "AverageRating" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRatings_RatedBySubscriptionId",
                table: "marketplace_ratings",
                column: "RatedBySubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "workout_exercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutId_Order",
                table: "workout_exercises",
                columns: new[] { "WorkoutId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutObjectives_ObjectiveId",
                table: "workout_objectives",
                column: "ObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_ObjectiveId",
                table: "workouts",
                column: "ObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_Ownership_IsActive",
                table: "workouts",
                columns: new[] { "Ownership", "IsActive" },
                filter: "subscription_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_SubscriptionId_IsActive",
                table: "workouts",
                columns: new[] { "SubscriptionId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise_objectives");

            migrationBuilder.DropTable(
                name: "marketplace_ratings");

            migrationBuilder.DropTable(
                name: "workout_exercises");

            migrationBuilder.DropTable(
                name: "workout_objectives");

            migrationBuilder.DropTable(
                name: "marketplace_items");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "workouts");

            migrationBuilder.DropTable(
                name: "exercise_categories");

            migrationBuilder.DropTable(
                name: "exercise_types");
        }
    }
}
