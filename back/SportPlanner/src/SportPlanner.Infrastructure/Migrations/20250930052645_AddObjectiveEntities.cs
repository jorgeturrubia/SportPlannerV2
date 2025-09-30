using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectiveEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GrantedBy",
                table: "SubscriptionUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "objective_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sport = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_objective_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "objective_subcategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectiveCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_objective_subcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_objective_subcategories_objective_categories_ObjectiveCateg~",
                        column: x => x.ObjectiveCategoryId,
                        principalTable: "objective_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "objectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Ownership = table.Column<string>(type: "text", nullable: false),
                    Sport = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ObjectiveCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectiveSubcategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SourceMarketplaceItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_objectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_objectives_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_objectives_objective_categories_ObjectiveCategoryId",
                        column: x => x.ObjectiveCategoryId,
                        principalTable: "objective_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_objectives_objective_subcategories_ObjectiveSubcategoryId",
                        column: x => x.ObjectiveSubcategoryId,
                        principalTable: "objective_subcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "objective_techniques",
                columns: table => new
                {
                    ObjectiveId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_objective_techniques", x => new { x.ObjectiveId, x.Id });
                    table.ForeignKey(
                        name: "FK_objective_techniques_objectives_ObjectiveId",
                        column: x => x.ObjectiveId,
                        principalTable: "objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111001"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111002"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111003"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111004"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111005"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111006"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111007"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111008"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222001"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222002"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222003"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222004"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222005"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333001"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333002"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333003"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333004"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "genders",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "genders",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "genders",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "objective_categories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Name", "Sport", "UpdatedAt", "UpdatedBy" },
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

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111102"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111103"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222201"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222202"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222203"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333301"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333302"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333303"),
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "objective_subcategories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Name", "ObjectiveCategoryId", "UpdatedAt", "UpdatedBy" },
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
                name: "IX_ObjectiveCategories_Sport",
                table: "objective_categories",
                column: "Sport");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveCategories_Sport_Name",
                table: "objective_categories",
                columns: new[] { "Sport", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveSubcategories_CategoryId_Name",
                table: "objective_subcategories",
                columns: new[] { "ObjectiveCategoryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveSubcategories_ObjectiveCategoryId",
                table: "objective_subcategories",
                column: "ObjectiveCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_ObjectiveCategoryId",
                table: "objectives",
                column: "ObjectiveCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_ObjectiveSubcategoryId",
                table: "objectives",
                column: "ObjectiveSubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_Ownership_Sport",
                table: "objectives",
                columns: new[] { "Ownership", "Sport" },
                filter: "subscription_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_SourceMarketplaceItemId",
                table: "objectives",
                column: "SourceMarketplaceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_SubscriptionId_Sport_IsActive",
                table: "objectives",
                columns: new[] { "SubscriptionId", "Sport", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "objective_techniques");

            migrationBuilder.DropTable(
                name: "objectives");

            migrationBuilder.DropTable(
                name: "objective_subcategories");

            migrationBuilder.DropTable(
                name: "objective_categories");

            migrationBuilder.AlterColumn<Guid>(
                name: "GrantedBy",
                table: "SubscriptionUsers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111001"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8490));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111002"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8498));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111003"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8501));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111004"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8504));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111005"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8506));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111006"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8508));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111007"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8511));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111008"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8513));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222001"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8524));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222002"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8527));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222003"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8529));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222004"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8531));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222005"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8533));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333001"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8539));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333002"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8541));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333003"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8543));

            migrationBuilder.UpdateData(
                table: "age_groups",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333004"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8546));

            migrationBuilder.UpdateData(
                table: "genders",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(7734));

            migrationBuilder.UpdateData(
                table: "genders",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(7744));

            migrationBuilder.UpdateData(
                table: "genders",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(7748));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8261));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111102"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8269));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111103"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8272));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8276));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222201"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8304));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222202"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8371));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222203"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8373));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333301"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8386));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333302"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8389));

            migrationBuilder.UpdateData(
                table: "team_categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333303"),
                column: "CreatedAt",
                value: new DateTime(2025, 9, 28, 16, 31, 21, 586, DateTimeKind.Utc).AddTicks(8392));
        }
    }
}
