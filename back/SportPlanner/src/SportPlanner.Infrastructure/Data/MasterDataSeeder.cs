using Microsoft.EntityFrameworkCore;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Infrastructure.Data;

public static class MasterDataSeeder
{
    private static readonly DateTime SeedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void SeedMasterData(ModelBuilder modelBuilder)
    {
        SeedGenders(modelBuilder);
        SeedTeamCategories(modelBuilder);
        SeedAgeGroups(modelBuilder);
        SeedObjectiveCategories(modelBuilder);
        SeedObjectiveSubcategories(modelBuilder);
    }

    private static void SeedGenders(ModelBuilder modelBuilder)
    {
        var genders = new[]
        {
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Masculino",
                Code = "M",
                Description = "Equipos masculinos",
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Femenino",
                Code = "F",
                Description = "Equipos femeninos",
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Mixto",
                Code = "X",
                Description = "Equipos de género mixto",
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        };

        modelBuilder.Entity<Gender>().HasData(genders);
    }

    private static void SeedTeamCategories(ModelBuilder modelBuilder)
    {
        var categories = new List<object>();

        // Football Categories
        categories.AddRange(new[]
        {
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                Name = "Nivel A",
                Code = "NIVEL_A",
                Description = "Categoría principal - máximo nivel competitivo",
                SortOrder = 1,
                Sport = Sport.Football,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                Name = "Nivel B",
                Code = "NIVEL_B",
                Description = "Segunda categoría - nivel competitivo medio",
                SortOrder = 2,
                Sport = Sport.Football,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111103"),
                Name = "Escuela",
                Code = "ESCUELA",
                Description = "Categoría de formación y aprendizaje",
                SortOrder = 3,
                Sport = Sport.Football,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111104"),
                Name = "Elite",
                Code = "ELITE",
                Description = "Categoría de élite - máximo rendimiento",
                SortOrder = 4,
                Sport = Sport.Football,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        // Basketball Categories
        categories.AddRange(new[]
        {
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222201"),
                Name = "Nivel A",
                Code = "NIVEL_A_BASKET",
                Description = "Categoría principal - máximo nivel competitivo",
                SortOrder = 1,
                Sport = Sport.Basketball,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222202"),
                Name = "Nivel B",
                Code = "NIVEL_B_BASKET",
                Description = "Segunda categoría - nivel competitivo medio",
                SortOrder = 2,
                Sport = Sport.Basketball,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222203"),
                Name = "Escuela",
                Code = "ESCUELA_BASKET",
                Description = "Categoría de formación y aprendizaje",
                SortOrder = 3,
                Sport = Sport.Basketball,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        // Handball Categories
        categories.AddRange(new[]
        {
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333301"),
                Name = "Nivel A",
                Code = "NIVEL_A_HANDBALL",
                Description = "Categoría principal - máximo nivel competitivo",
                SortOrder = 1,
                Sport = Sport.Handball,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333302"),
                Name = "Nivel B",
                Code = "NIVEL_B_HANDBALL",
                Description = "Segunda categoría - nivel competitivo medio",
                SortOrder = 2,
                Sport = Sport.Handball,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333303"),
                Name = "Escuela",
                Code = "ESCUELA_HANDBALL",
                Description = "Categoría de formación y aprendizaje",
                SortOrder = 3,
                Sport = Sport.Handball,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        modelBuilder.Entity<TeamCategory>().HasData(categories);
    }

    private static void SeedAgeGroups(ModelBuilder modelBuilder)
    {
        var ageGroups = new List<object>();

        // Football Age Groups
        ageGroups.AddRange(new[]
        {
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111001"),
                Name = "Alevín",
                Code = "ALEVIN_FOOTBALL",
                MinAge = 8,
                MaxAge = 10,
                Sport = Sport.Football,
                SortOrder = 1,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111002"),
                Name = "Benjamín",
                Code = "BENJAMIN_FOOTBALL",
                MinAge = 11,
                MaxAge = 12,
                Sport = Sport.Football,
                SortOrder = 2,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111003"),
                Name = "Infantil",
                Code = "INFANTIL_FOOTBALL",
                MinAge = 13,
                MaxAge = 14,
                Sport = Sport.Football,
                SortOrder = 3,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111004"),
                Name = "Cadete",
                Code = "CADETE_FOOTBALL",
                MinAge = 15,
                MaxAge = 16,
                Sport = Sport.Football,
                SortOrder = 4,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111005"),
                Name = "Juvenil",
                Code = "JUVENIL_FOOTBALL",
                MinAge = 17,
                MaxAge = 18,
                Sport = Sport.Football,
                SortOrder = 5,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111006"),
                Name = "Junior",
                Code = "JUNIOR_FOOTBALL",
                MinAge = 19,
                MaxAge = 20,
                Sport = Sport.Football,
                SortOrder = 6,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111007"),
                Name = "Senior",
                Code = "SENIOR_FOOTBALL",
                MinAge = 21,
                MaxAge = 34,
                Sport = Sport.Football,
                SortOrder = 7,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111008"),
                Name = "Veterano",
                Code = "VETERANO_FOOTBALL",
                MinAge = 35,
                MaxAge = 50,
                Sport = Sport.Football,
                SortOrder = 8,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        // Basketball Age Groups (similar structure)
        ageGroups.AddRange(new[]
        {
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222001"),
                Name = "Mini",
                Code = "MINI_BASKETBALL",
                MinAge = 8,
                MaxAge = 10,
                Sport = Sport.Basketball,
                SortOrder = 1,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222002"),
                Name = "Infantil",
                Code = "INFANTIL_BASKETBALL",
                MinAge = 11,
                MaxAge = 13,
                Sport = Sport.Basketball,
                SortOrder = 2,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222003"),
                Name = "Cadete",
                Code = "CADETE_BASKETBALL",
                MinAge = 14,
                MaxAge = 16,
                Sport = Sport.Basketball,
                SortOrder = 3,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222004"),
                Name = "Juvenil",
                Code = "JUVENIL_BASKETBALL",
                MinAge = 17,
                MaxAge = 18,
                Sport = Sport.Basketball,
                SortOrder = 4,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222005"),
                Name = "Senior",
                Code = "SENIOR_BASKETBALL",
                MinAge = 19,
                MaxAge = 40,
                Sport = Sport.Basketball,
                SortOrder = 5,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        // Handball Age Groups (similar structure)
        ageGroups.AddRange(new[]
        {
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333001"),
                Name = "Infantil",
                Code = "INFANTIL_HANDBALL",
                MinAge = 10,
                MaxAge = 12,
                Sport = Sport.Handball,
                SortOrder = 1,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333002"),
                Name = "Cadete",
                Code = "CADETE_HANDBALL",
                MinAge = 13,
                MaxAge = 15,
                Sport = Sport.Handball,
                SortOrder = 2,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333003"),
                Name = "Juvenil",
                Code = "JUVENIL_HANDBALL",
                MinAge = 16,
                MaxAge = 18,
                Sport = Sport.Handball,
                SortOrder = 3,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333004"),
                Name = "Senior",
                Code = "SENIOR_HANDBALL",
                MinAge = 19,
                MaxAge = 40,
                Sport = Sport.Handball,
                SortOrder = 4,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        modelBuilder.Entity<AgeGroup>().HasData(ageGroups);
    }

    private static void SeedObjectiveCategories(ModelBuilder modelBuilder)
    {
        var categories = new List<object>();

        // Football Categories
        categories.AddRange(new[]
        {
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444401"),
                Name = "Técnica Individual",
                Sport = Sport.Football,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444402"),
                Name = "Técnica Colectiva",
                Sport = Sport.Football,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444403"),
                Name = "Táctica",
                Sport = Sport.Football,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444404"),
                Name = "Física",
                Sport = Sport.Football,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        // Basketball Categories
        categories.AddRange(new[]
        {
            new
            {
                Id = Guid.Parse("55555501-5555-5555-5555-555555555555"),
                Name = "Técnica Individual",
                Sport = Sport.Basketball,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("55555502-5555-5555-5555-555555555555"),
                Name = "Técnica Colectiva",
                Sport = Sport.Basketball,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("55555503-5555-5555-5555-555555555555"),
                Name = "Táctica",
                Sport = Sport.Basketball,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("55555504-5555-5555-5555-555555555555"),
                Name = "Física",
                Sport = Sport.Basketball,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        // Handball Categories
        categories.AddRange(new[]
        {
            new
            {
                Id = Guid.Parse("66666601-6666-6666-6666-666666666666"),
                Name = "Técnica Individual",
                Sport = Sport.Handball,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("66666602-6666-6666-6666-666666666666"),
                Name = "Técnica Colectiva",
                Sport = Sport.Handball,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("66666603-6666-6666-6666-666666666666"),
                Name = "Táctica",
                Sport = Sport.Handball,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("66666604-6666-6666-6666-666666666666"),
                Name = "Física",
                Sport = Sport.Handball,
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        modelBuilder.Entity<ObjectiveCategory>().HasData(categories);
    }

    private static void SeedObjectiveSubcategories(ModelBuilder modelBuilder)
    {
        var subcategories = new List<object>();

        // Football Subcategories - For Técnica Individual, Técnica Colectiva, Táctica
        subcategories.AddRange(new[]
        {
            // Técnica Individual
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444440101"),
                ObjectiveCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444401"),
                Name = "Ataque",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444440102"),
                ObjectiveCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444401"),
                Name = "Defensa",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            // Técnica Colectiva
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444440201"),
                ObjectiveCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444402"),
                Name = "Ataque",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444440202"),
                ObjectiveCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444402"),
                Name = "Defensa",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            // Táctica
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444440301"),
                ObjectiveCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444403"),
                Name = "Ataque",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444440302"),
                ObjectiveCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444403"),
                Name = "Defensa",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444440303"),
                ObjectiveCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444403"),
                Name = "Transición",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        // Basketball Subcategories
        subcategories.AddRange(new[]
        {
            // Técnica Individual
            new
            {
                Id = Guid.Parse("55550101-5555-5555-5555-555555555555"),
                ObjectiveCategoryId = Guid.Parse("55555501-5555-5555-5555-555555555555"),
                Name = "Ataque",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("55550102-5555-5555-5555-555555555555"),
                ObjectiveCategoryId = Guid.Parse("55555501-5555-5555-5555-555555555555"),
                Name = "Defensa",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            // Técnica Colectiva
            new
            {
                Id = Guid.Parse("55550201-5555-5555-5555-555555555555"),
                ObjectiveCategoryId = Guid.Parse("55555502-5555-5555-5555-555555555555"),
                Name = "Ataque",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("55550202-5555-5555-5555-555555555555"),
                ObjectiveCategoryId = Guid.Parse("55555502-5555-5555-5555-555555555555"),
                Name = "Defensa",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            // Táctica
            new
            {
                Id = Guid.Parse("55550301-5555-5555-5555-555555555555"),
                ObjectiveCategoryId = Guid.Parse("55555503-5555-5555-5555-555555555555"),
                Name = "Ataque",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("55550302-5555-5555-5555-555555555555"),
                ObjectiveCategoryId = Guid.Parse("55555503-5555-5555-5555-555555555555"),
                Name = "Defensa",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("55550303-5555-5555-5555-555555555555"),
                ObjectiveCategoryId = Guid.Parse("55555503-5555-5555-5555-555555555555"),
                Name = "Transición",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        // Handball Subcategories
        subcategories.AddRange(new[]
        {
            // Técnica Individual
            new
            {
                Id = Guid.Parse("66660101-6666-6666-6666-666666666666"),
                ObjectiveCategoryId = Guid.Parse("66666601-6666-6666-6666-666666666666"),
                Name = "Ataque",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("66660102-6666-6666-6666-666666666666"),
                ObjectiveCategoryId = Guid.Parse("66666601-6666-6666-6666-666666666666"),
                Name = "Defensa",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            // Técnica Colectiva
            new
            {
                Id = Guid.Parse("66660201-6666-6666-6666-666666666666"),
                ObjectiveCategoryId = Guid.Parse("66666602-6666-6666-6666-666666666666"),
                Name = "Ataque",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("66660202-6666-6666-6666-666666666666"),
                ObjectiveCategoryId = Guid.Parse("66666602-6666-6666-6666-666666666666"),
                Name = "Defensa",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            // Táctica
            new
            {
                Id = Guid.Parse("66660301-6666-6666-6666-666666666666"),
                ObjectiveCategoryId = Guid.Parse("66666603-6666-6666-6666-666666666666"),
                Name = "Ataque",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("66660302-6666-6666-6666-666666666666"),
                ObjectiveCategoryId = Guid.Parse("66666603-6666-6666-6666-666666666666"),
                Name = "Defensa",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("66660303-6666-6666-6666-666666666666"),
                ObjectiveCategoryId = Guid.Parse("66666603-6666-6666-6666-666666666666"),
                Name = "Transición",
                CreatedAt = SeedDate,
                CreatedBy = "System"
            }
        });

        modelBuilder.Entity<ObjectiveSubcategory>().HasData(subcategories);
    }
}