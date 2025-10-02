using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "age_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    min_age = table.Column<int>(type: "integer", nullable: false),
                    max_age = table.Column<int>(type: "integer", nullable: false),
                    sport = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_age_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exercise_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sport = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
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
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    requires_sets = table.Column<bool>(type: "boolean", nullable: false),
                    requires_reps = table.Column<bool>(type: "boolean", nullable: false),
                    requires_duration = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercise_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "objective_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sport = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_objective_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    sport = table.Column<int>(type: "integer", nullable: false),
                    max_users = table.Column<int>(type: "integer", nullable: false),
                    max_teams = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "team_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sport = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "objective_subcategories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    objective_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_objective_subcategories", x => x.id);
                    table.ForeignKey(
                        name: "fk_objective_subcategories_objective_categories_objective_cate",
                        column: x => x.objective_category_id,
                        principalTable: "objective_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ownership = table.Column<string>(type: "text", nullable: false),
                    marketplace_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    instructions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    default_sets = table.Column<int>(type: "integer", nullable: true),
                    default_reps = table.Column<int>(type: "integer", nullable: true),
                    default_duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    default_intensity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercises", x => x.id);
                    table.ForeignKey(
                        name: "fk_exercises_exercise_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "exercise_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_exercises_exercise_types_type_id",
                        column: x => x.type_id,
                        principalTable: "exercise_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_exercises_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "marketplace_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    sport = table.Column<string>(type: "text", nullable: false),
                    source_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_ownership = table.Column<string>(type: "text", nullable: false),
                    published_by_subscription_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    is_system_official = table.Column<bool>(type: "boolean", nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    total_ratings = table.Column<int>(type: "integer", nullable: false),
                    total_downloads = table.Column<int>(type: "integer", nullable: false),
                    total_views = table.Column<int>(type: "integer", nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_marketplace_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_marketplace_items_subscriptions_published_by_subscription_id",
                        column: x => x.published_by_subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_in_subscription = table.Column<int>(type: "integer", nullable: false),
                    granted_by = table.Column<string>(type: "text", nullable: false),
                    granted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    removed_by = table.Column<string>(type: "text", nullable: true),
                    removed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscription_users_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training_plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    training_days = table.Column<string>(type: "text", nullable: false),
                    hours_per_day = table.Column<string>(type: "text", nullable: false),
                    total_weeks = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    marketplace_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_training_plans", x => x.id);
                    table.ForeignKey(
                        name: "fk_training_plans_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    age_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    color = table.Column<string>(type: "text", nullable: false),
                    sport = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    home_venue = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    coach_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    contact_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    contact_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    season = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    max_players = table.Column<int>(type: "integer", nullable: false),
                    current_players_count = table.Column<int>(type: "integer", nullable: false),
                    last_match_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    allow_mixed_gender = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                    table.ForeignKey(
                        name: "fk_teams_age_groups_age_group_id",
                        column: x => x.age_group_id,
                        principalTable: "age_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_teams_genders_gender_id",
                        column: x => x.gender_id,
                        principalTable: "genders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_teams_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_teams_team_categories_team_category_id",
                        column: x => x.team_category_id,
                        principalTable: "team_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "objectives",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ownership = table.Column<string>(type: "text", nullable: false),
                    sport = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    objective_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    objective_subcategory_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    source_marketplace_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_objectives", x => x.id);
                    table.ForeignKey(
                        name: "fk_objectives_objective_categories_objective_category_id",
                        column: x => x.objective_category_id,
                        principalTable: "objective_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_objectives_objective_subcategories_objective_subcategory_id",
                        column: x => x.objective_subcategory_id,
                        principalTable: "objective_subcategories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_objectives_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "marketplace_ratings",
                columns: table => new
                {
                    marketplace_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rated_by_subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stars = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_marketplace_ratings", x => new { x.marketplace_item_id, x.rated_by_subscription_id });
                    table.ForeignKey(
                        name: "fk_marketplace_ratings_marketplace_items_marketplace_item_id",
                        column: x => x.marketplace_item_id,
                        principalTable: "marketplace_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_marketplace_ratings_subscriptions_rated_by_subscription_id",
                        column: x => x.rated_by_subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exercise_objectives",
                columns: table => new
                {
                    exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                    objective_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercise_objectives", x => new { x.exercise_id, x.objective_id });
                    table.ForeignKey(
                        name: "fk_exercise_objectives_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_exercise_objectives_objectives_objective_id",
                        column: x => x.objective_id,
                        principalTable: "objectives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "objective_techniques",
                columns: table => new
                {
                    objective_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_objective_techniques", x => new { x.objective_id, x.id });
                    table.ForeignKey(
                        name: "fk_objective_techniques_objectives_objective_id",
                        column: x => x.objective_id,
                        principalTable: "objectives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_objectives",
                columns: table => new
                {
                    training_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    objective_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    target_sessions = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_objectives", x => new { x.training_plan_id, x.objective_id });
                    table.ForeignKey(
                        name: "fk_plan_objectives_objectives_objective_id",
                        column: x => x.objective_id,
                        principalTable: "objectives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_plan_objectives_training_plans_training_plan_id",
                        column: x => x.training_plan_id,
                        principalTable: "training_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workouts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ownership = table.Column<string>(type: "text", nullable: false),
                    marketplace_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    objective_id = table.Column<Guid>(type: "uuid", nullable: true),
                    estimated_duration_minutes = table.Column<int>(type: "integer", nullable: true),
                    difficulty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workouts", x => x.id);
                    table.ForeignKey(
                        name: "fk_workouts_objectives_objective_id",
                        column: x => x.objective_id,
                        principalTable: "objectives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_workouts_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_exercises",
                columns: table => new
                {
                    workout_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    sets = table.Column<int>(type: "integer", nullable: true),
                    reps = table.Column<int>(type: "integer", nullable: true),
                    duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    intensity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    rest_seconds = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workout_exercises", x => new { x.workout_id, x.exercise_id });
                    table.ForeignKey(
                        name: "fk_workout_exercises_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_workout_exercises_workouts_workout_id",
                        column: x => x.workout_id,
                        principalTable: "workouts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_objectives",
                columns: table => new
                {
                    workout_id = table.Column<Guid>(type: "uuid", nullable: false),
                    objective_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workout_objectives", x => new { x.workout_id, x.objective_id });
                    table.ForeignKey(
                        name: "fk_workout_objectives_objectives_objective_id",
                        column: x => x.objective_id,
                        principalTable: "objectives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workout_objectives_workouts_workout_id",
                        column: x => x.workout_id,
                        principalTable: "workouts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "age_groups",
                columns: new[] { "id", "code", "created_at", "created_by", "is_active", "max_age", "min_age", "name", "sort_order", "sport", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111001"), "ALEVIN_FOOTBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 10, 8, "Alevín", 1, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111002"), "BENJAMIN_FOOTBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 12, 11, "Benjamín", 2, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111003"), "INFANTIL_FOOTBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 14, 13, "Infantil", 3, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111004"), "CADETE_FOOTBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 16, 15, "Cadete", 4, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111005"), "JUVENIL_FOOTBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 18, 17, "Juvenil", 5, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111006"), "JUNIOR_FOOTBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 20, 19, "Junior", 6, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111007"), "SENIOR_FOOTBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 34, 21, "Senior", 7, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111008"), "VETERANO_FOOTBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 50, 35, "Veterano", 8, "Football", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222001"), "MINI_BASKETBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 10, 8, "Mini", 1, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222002"), "INFANTIL_BASKETBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 13, 11, "Infantil", 2, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222003"), "CADETE_BASKETBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 16, 14, "Cadete", 3, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222004"), "JUVENIL_BASKETBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 18, 17, "Juvenil", 4, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222005"), "SENIOR_BASKETBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 40, 19, "Senior", 5, "Basketball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333001"), "INFANTIL_HANDBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 12, 10, "Infantil", 1, "Handball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333002"), "CADETE_HANDBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 15, 13, "Cadete", 2, "Handball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333003"), "JUVENIL_HANDBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 18, 16, "Juvenil", 3, "Handball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333004"), "SENIOR_HANDBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 40, 19, "Senior", 4, "Handball", null, null }
                });

            migrationBuilder.InsertData(
                table: "genders",
                columns: new[] { "id", "code", "created_at", "created_by", "description", "is_active", "name", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "M", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Equipos masculinos", true, "Masculino", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "F", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Equipos femeninos", true, "Femenino", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "X", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Equipos de género mixto", true, "Mixto", null, null }
                });

            migrationBuilder.InsertData(
                table: "objective_categories",
                columns: new[] { "id", "created_at", "created_by", "name", "sport", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444401"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Técnica Individual", "Football", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444402"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Técnica Colectiva", "Football", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444403"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Táctica", "Football", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444404"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Física", "Football", null, null },
                    { new Guid("55555501-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Técnica Individual", "Basketball", null, null },
                    { new Guid("55555502-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Técnica Colectiva", "Basketball", null, null },
                    { new Guid("55555503-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Táctica", "Basketball", null, null },
                    { new Guid("55555504-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Física", "Basketball", null, null },
                    { new Guid("66666601-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Técnica Individual", "Handball", null, null },
                    { new Guid("66666602-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Técnica Colectiva", "Handball", null, null },
                    { new Guid("66666603-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Táctica", "Handball", null, null },
                    { new Guid("66666604-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Física", "Handball", null, null }
                });

            migrationBuilder.InsertData(
                table: "team_categories",
                columns: new[] { "id", "code", "created_at", "created_by", "description", "is_active", "name", "sort_order", "sport", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), "NIVEL_A", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Categoría principal - máximo nivel competitivo", true, "Nivel A", 1, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111102"), "NIVEL_B", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Segunda categoría - nivel competitivo medio", true, "Nivel B", 2, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111103"), "ESCUELA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Categoría de formación y aprendizaje", true, "Escuela", 3, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111104"), "ELITE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Categoría de élite - máximo rendimiento", true, "Elite", 4, "Football", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222201"), "NIVEL_A_BASKET", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Categoría principal - máximo nivel competitivo", true, "Nivel A", 1, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222202"), "NIVEL_B_BASKET", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Segunda categoría - nivel competitivo medio", true, "Nivel B", 2, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222203"), "ESCUELA_BASKET", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Categoría de formación y aprendizaje", true, "Escuela", 3, "Basketball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333301"), "NIVEL_A_HANDBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Categoría principal - máximo nivel competitivo", true, "Nivel A", 1, "Handball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333302"), "NIVEL_B_HANDBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Segunda categoría - nivel competitivo medio", true, "Nivel B", 2, "Handball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333303"), "ESCUELA_HANDBALL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Categoría de formación y aprendizaje", true, "Escuela", 3, "Handball", null, null }
                });

            migrationBuilder.InsertData(
                table: "objective_subcategories",
                columns: new[] { "id", "created_at", "created_by", "name", "objective_category_id", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444440101"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ataque", new Guid("44444444-4444-4444-4444-444444444401"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440102"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Defensa", new Guid("44444444-4444-4444-4444-444444444401"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440201"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ataque", new Guid("44444444-4444-4444-4444-444444444402"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440202"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Defensa", new Guid("44444444-4444-4444-4444-444444444402"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440301"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ataque", new Guid("44444444-4444-4444-4444-444444444403"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440302"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Defensa", new Guid("44444444-4444-4444-4444-444444444403"), null, null },
                    { new Guid("44444444-4444-4444-4444-444444440303"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Transición", new Guid("44444444-4444-4444-4444-444444444403"), null, null },
                    { new Guid("55550101-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ataque", new Guid("55555501-5555-5555-5555-555555555555"), null, null },
                    { new Guid("55550102-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Defensa", new Guid("55555501-5555-5555-5555-555555555555"), null, null },
                    { new Guid("55550201-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ataque", new Guid("55555502-5555-5555-5555-555555555555"), null, null },
                    { new Guid("55550202-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Defensa", new Guid("55555502-5555-5555-5555-555555555555"), null, null },
                    { new Guid("55550301-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ataque", new Guid("55555503-5555-5555-5555-555555555555"), null, null },
                    { new Guid("55550302-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Defensa", new Guid("55555503-5555-5555-5555-555555555555"), null, null },
                    { new Guid("55550303-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Transición", new Guid("55555503-5555-5555-5555-555555555555"), null, null },
                    { new Guid("66660101-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ataque", new Guid("66666601-6666-6666-6666-666666666666"), null, null },
                    { new Guid("66660102-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Defensa", new Guid("66666601-6666-6666-6666-666666666666"), null, null },
                    { new Guid("66660201-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ataque", new Guid("66666602-6666-6666-6666-666666666666"), null, null },
                    { new Guid("66660202-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Defensa", new Guid("66666602-6666-6666-6666-666666666666"), null, null },
                    { new Guid("66660301-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ataque", new Guid("66666603-6666-6666-6666-666666666666"), null, null },
                    { new Guid("66660302-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Defensa", new Guid("66666603-6666-6666-6666-666666666666"), null, null },
                    { new Guid("66660303-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Transición", new Guid("66666603-6666-6666-6666-666666666666"), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "ix_age_groups_code",
                table: "age_groups",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_age_groups_is_active",
                table: "age_groups",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_age_groups_sport_min_age_max_age",
                table: "age_groups",
                columns: new[] { "sport", "min_age", "max_age" });

            migrationBuilder.CreateIndex(
                name: "ix_age_groups_sport_sort_order",
                table: "age_groups",
                columns: new[] { "sport", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseCategories_Sport_IsActive",
                table: "exercise_categories",
                columns: new[] { "sport", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseObjectives_ObjectiveId",
                table: "exercise_objectives",
                column: "objective_id");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTypes_IsActive",
                table: "exercise_types",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_CategoryId",
                table: "exercises",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_Ownership_IsActive",
                table: "exercises",
                columns: new[] { "ownership", "is_active" },
                filter: "subscription_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_SubscriptionId_IsActive",
                table: "exercises",
                columns: new[] { "subscription_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_TypeId",
                table: "exercises",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_genders_code",
                table: "genders",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_genders_is_active",
                table: "genders",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_PublishedBySubscriptionId",
                table: "marketplace_items",
                column: "published_by_subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_SourceEntityId",
                table: "marketplace_items",
                column: "source_entity_id");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_Sport_Downloads",
                table: "marketplace_items",
                columns: new[] { "sport", "total_downloads" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_Sport_Official_Rating",
                table: "marketplace_items",
                columns: new[] { "sport", "is_system_official", "average_rating" },
                descending: new[] { false, false, true },
                filter: "is_system_official = true");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_Sport_Type_Rating",
                table: "marketplace_items",
                columns: new[] { "sport", "type", "average_rating" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceRatings_RatedBySubscriptionId",
                table: "marketplace_ratings",
                column: "rated_by_subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveCategories_Sport",
                table: "objective_categories",
                column: "sport");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveCategories_Sport_Name",
                table: "objective_categories",
                columns: new[] { "sport", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveSubcategories_CategoryId_Name",
                table: "objective_subcategories",
                columns: new[] { "objective_category_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveSubcategories_ObjectiveCategoryId",
                table: "objective_subcategories",
                column: "objective_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_ObjectiveCategoryId",
                table: "objectives",
                column: "objective_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_ObjectiveSubcategoryId",
                table: "objectives",
                column: "objective_subcategory_id");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_Ownership_Sport",
                table: "objectives",
                columns: new[] { "ownership", "sport" },
                filter: "subscription_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_SourceMarketplaceItemId",
                table: "objectives",
                column: "source_marketplace_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_SubscriptionId_Sport_IsActive",
                table: "objectives",
                columns: new[] { "subscription_id", "sport", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanObjectives_ObjectiveId",
                table: "plan_objectives",
                column: "objective_id");

            migrationBuilder.CreateIndex(
                name: "IX_PlanObjectives_TrainingPlanId",
                table: "plan_objectives",
                column: "training_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_PlanObjectives_TrainingPlanId_Priority",
                table: "plan_objectives",
                columns: new[] { "training_plan_id", "priority" });

            migrationBuilder.CreateIndex(
                name: "ix_subscription_users_subscription_id_removed_at",
                table: "subscription_users",
                columns: new[] { "subscription_id", "removed_at" },
                filter: "removed_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_users_subscription_id_user_id",
                table: "subscription_users",
                columns: new[] { "subscription_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_owner_id",
                table: "subscriptions",
                column: "owner_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_team_categories_code",
                table: "team_categories",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_team_categories_is_active",
                table: "team_categories",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_team_categories_sport_sort_order",
                table: "team_categories",
                columns: new[] { "sport", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_AgeGroupId",
                table: "teams",
                column: "age_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_GenderId",
                table: "teams",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_IsActive",
                table: "teams",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SubscriptionId",
                table: "teams",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SubscriptionId_IsActive",
                table: "teams",
                columns: new[] { "subscription_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SubscriptionId_Name",
                table: "teams",
                columns: new[] { "subscription_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamCategoryId",
                table: "teams",
                column: "team_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlans_MarketplaceItemId",
                table: "training_plans",
                column: "marketplace_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlans_SubscriptionId",
                table: "training_plans",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlans_SubscriptionId_IsActive",
                table: "training_plans",
                columns: new[] { "subscription_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "workout_exercises",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutId_Order",
                table: "workout_exercises",
                columns: new[] { "workout_id", "order" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutObjectives_ObjectiveId",
                table: "workout_objectives",
                column: "objective_id");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_ObjectiveId",
                table: "workouts",
                column: "objective_id");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_Ownership_IsActive",
                table: "workouts",
                columns: new[] { "ownership", "is_active" },
                filter: "subscription_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_SubscriptionId_IsActive",
                table: "workouts",
                columns: new[] { "subscription_id", "is_active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise_objectives");

            migrationBuilder.DropTable(
                name: "marketplace_ratings");

            migrationBuilder.DropTable(
                name: "objective_techniques");

            migrationBuilder.DropTable(
                name: "plan_objectives");

            migrationBuilder.DropTable(
                name: "subscription_users");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "workout_exercises");

            migrationBuilder.DropTable(
                name: "workout_objectives");

            migrationBuilder.DropTable(
                name: "marketplace_items");

            migrationBuilder.DropTable(
                name: "training_plans");

            migrationBuilder.DropTable(
                name: "age_groups");

            migrationBuilder.DropTable(
                name: "genders");

            migrationBuilder.DropTable(
                name: "team_categories");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "workouts");

            migrationBuilder.DropTable(
                name: "exercise_categories");

            migrationBuilder.DropTable(
                name: "exercise_types");

            migrationBuilder.DropTable(
                name: "objectives");

            migrationBuilder.DropTable(
                name: "objective_subcategories");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "objective_categories");
        }
    }
}
