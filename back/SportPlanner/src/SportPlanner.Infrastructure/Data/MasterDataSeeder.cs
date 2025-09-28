using Microsoft.EntityFrameworkCore;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Infrastructure.Data;

public static class MasterDataSeeder
{
    public static void SeedMasterData(ModelBuilder modelBuilder)
    {
        SeedGenders(modelBuilder);
        SeedTeamCategories(modelBuilder);
        SeedAgeGroups(modelBuilder);
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
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Femenino",
                Code = "F",
                Description = "Equipos femeninos",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Mixto",
                Code = "X",
                Description = "Equipos de género mixto",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        });

        modelBuilder.Entity<AgeGroup>().HasData(ageGroups);
    }
}