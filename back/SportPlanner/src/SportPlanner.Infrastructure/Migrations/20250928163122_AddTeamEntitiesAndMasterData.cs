using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamEntitiesAndMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RemovedBy",
                table: "SubscriptionUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "age_groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MinAge = table.Column<int>(type: "integer", nullable: false),
                    MaxAge = table.Column<int>(type: "integer", nullable: false),
                    Sport = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_age_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "genders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "team_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Sport = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgeGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Sport = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    HomeVenue = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CoachName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Season = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxPlayers = table.Column<int>(type: "integer", nullable: false),
                    CurrentPlayersCount = table.Column<int>(type: "integer", nullable: false),
                    LastMatchDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AllowMixedGender = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teams_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teams_age_groups_AgeGroupId",
                        column: x => x.AgeGroupId,
                        principalTable: "age_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_teams_genders_GenderId",
                        column: x => x.GenderId,
                        principalTable: "genders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_teams_team_categories_TeamCategoryId",
                        column: x => x.TeamCategoryId,
                        principalTable: "team_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "age_groups",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "IsActive", "MaxAge", "MinAge", "Name", "SortOrder", "Sport", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111001"), "ALEVIN_FOOTBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8490), "System", true, 10, 8, "Alevín", 1, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111002"), "BENJAMIN_FOOTBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8498), "System", true, 12, 11, "Benjamín", 2, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111003"), "INFANTIL_FOOTBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8501), "System", true, 14, 13, "Infantil", 3, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111004"), "CADETE_FOOTBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8504), "System", true, 16, 15, "Cadete", 4, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111005"), "JUVENIL_FOOTBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8506), "System", true, 18, 17, "Juvenil", 5, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111006"), "JUNIOR_FOOTBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8508), "System", true, 20, 19, "Junior", 6, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111007"), "SENIOR_FOOTBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8511), "System", true, 34, 21, "Senior", 7, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111008"), "VETERANO_FOOTBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8513), "System", true, 50, 35, "Veterano", 8, "Football", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222001"), "MINI_BASKETBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8524), "System", true, 10, 8, "Mini", 1, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222002"), "INFANTIL_BASKETBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8527), "System", true, 13, 11, "Infantil", 2, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222003"), "CADETE_BASKETBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8529), "System", true, 16, 14, "Cadete", 3, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222004"), "JUVENIL_BASKETBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8531), "System", true, 18, 17, "Juvenil", 4, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222005"), "SENIOR_BASKETBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8533), "System", true, 40, 19, "Senior", 5, "Basketball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333001"), "INFANTIL_HANDBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8539), "System", true, 12, 10, "Infantil", 1, "Handball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333002"), "CADETE_HANDBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8541), "System", true, 15, 13, "Cadete", 2, "Handball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333003"), "JUVENIL_HANDBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8543), "System", true, 18, 16, "Juvenil", 3, "Handball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333004"), "SENIOR_HANDBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8546), "System", true, 40, 19, "Senior", 4, "Handball", null, null }
                });

            migrationBuilder.InsertData(
                table: "genders",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Description", "IsActive", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "M", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(7734), "System", "Equipos masculinos", true, "Masculino", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "F", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(7744), "System", "Equipos femeninos", true, "Femenino", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "X", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(7748), "System", "Equipos de género mixto", true, "Mixto", null, null }
                });

            migrationBuilder.InsertData(
                table: "team_categories",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Description", "IsActive", "Name", "SortOrder", "Sport", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), "NIVEL_A", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8261), "System", "Categoría principal - máximo nivel competitivo", true, "Nivel A", 1, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111102"), "NIVEL_B", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8269), "System", "Segunda categoría - nivel competitivo medio", true, "Nivel B", 2, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111103"), "ESCUELA", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8272), "System", "Categoría de formación y aprendizaje", true, "Escuela", 3, "Football", null, null },
                    { new Guid("11111111-1111-1111-1111-111111111104"), "ELITE", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8276), "System", "Categoría de élite - máximo rendimiento", true, "Elite", 4, "Football", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222201"), "NIVEL_A_BASKET", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8304), "System", "Categoría principal - máximo nivel competitivo", true, "Nivel A", 1, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222202"), "NIVEL_B_BASKET", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8371), "System", "Segunda categoría - nivel competitivo medio", true, "Nivel B", 2, "Basketball", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222203"), "ESCUELA_BASKET", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8373), "System", "Categoría de formación y aprendizaje", true, "Escuela", 3, "Basketball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333301"), "NIVEL_A_HANDBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8386), "System", "Categoría principal - máximo nivel competitivo", true, "Nivel A", 1, "Handball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333302"), "NIVEL_B_HANDBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8389), "System", "Segunda categoría - nivel competitivo medio", true, "Nivel B", 2, "Handball", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333303"), "ESCUELA_HANDBALL", new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8392), "System", "Categoría de formación y aprendizaje", true, "Escuela", 3, "Handball", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_age_groups_Code",
                table: "age_groups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_age_groups_IsActive",
                table: "age_groups",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_age_groups_Sport_MinAge_MaxAge",
                table: "age_groups",
                columns: new[] { "Sport", "MinAge", "MaxAge" });

            migrationBuilder.CreateIndex(
                name: "IX_age_groups_Sport_SortOrder",
                table: "age_groups",
                columns: new[] { "Sport", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_genders_Code",
                table: "genders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_genders_IsActive",
                table: "genders",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_team_categories_Code",
                table: "team_categories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_team_categories_IsActive",
                table: "team_categories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_team_categories_Sport_SortOrder",
                table: "team_categories",
                columns: new[] { "Sport", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_AgeGroupId",
                table: "teams",
                column: "AgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_GenderId",
                table: "teams",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_IsActive",
                table: "teams",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SubscriptionId",
                table: "teams",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SubscriptionId_IsActive",
                table: "teams",
                columns: new[] { "SubscriptionId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SubscriptionId_Name",
                table: "teams",
                columns: new[] { "SubscriptionId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamCategoryId",
                table: "teams",
                column: "TeamCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "age_groups");

            migrationBuilder.DropTable(
                name: "genders");

            migrationBuilder.DropTable(
                name: "team_categories");

            migrationBuilder.AlterColumn<Guid>(
                name: "RemovedBy",
                table: "SubscriptionUsers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
