using SportPlanner.Domain.Enum;
using SportPlanner.Domain.Interfaces;

namespace SportPlanner.Domain.Entities;

public class Team : Entity, IAuditable
{
    // Relaciones principales
    public Guid SubscriptionId { get; private set; }
    public Guid TeamCategoryId { get; private set; }
    public Guid GenderId { get; private set; }
    public Guid AgeGroupId { get; private set; }
    public Guid? CoachSubscriptionUserId { get; private set; }

    // Propiedades directas
    public string Name { get; private set; }
    public TeamColor Color { get; private set; }
    public Sport Sport { get; private set; }
    public bool IsActive { get; private set; }

    // Propiedades adicionales
    public string? Description { get; private set; }
    public DateTime? Season { get; private set; }
    public int MaxPlayers { get; private set; }
    public int CurrentPlayersCount { get; private set; }
    public DateTime? LastMatchDate { get; private set; }
    public bool AllowMixedGender { get; private set; }

    // Navigation properties
    public virtual TeamCategory Category { get; private set; } = null!;
    public virtual Gender Gender { get; private set; } = null!;
    public virtual AgeGroup AgeGroup { get; private set; } = null!;
    public virtual SubscriptionUser? Coach { get; private set; }

    // IAuditable
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // For EF Core
    private Team() { }

    public Team(
        Guid subscriptionId,
        string name,
        TeamColor color,
        Guid teamCategoryId,
        Guid genderId,
        Guid ageGroupId,
        Sport sport,
        string? description = null,
        Guid? coachSubscriptionUserId = null,
        DateTime? season = null,
        bool allowMixedGender = false)
    {
        if (subscriptionId == Guid.Empty)
            throw new ArgumentException("SubscriptionId cannot be empty", nameof(subscriptionId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (teamCategoryId == Guid.Empty)
            throw new ArgumentException("TeamCategoryId cannot be empty", nameof(teamCategoryId));

        if (genderId == Guid.Empty)
            throw new ArgumentException("GenderId cannot be empty", nameof(genderId));

        if (ageGroupId == Guid.Empty)
            throw new ArgumentException("AgeGroupId cannot be empty", nameof(ageGroupId));

        SubscriptionId = subscriptionId;
        Name = name;
        Color = color;
        TeamCategoryId = teamCategoryId;
        GenderId = genderId;
        AgeGroupId = ageGroupId;
        CoachSubscriptionUserId = coachSubscriptionUserId;
        Sport = sport;
        Description = description;
        Season = season;
        AllowMixedGender = allowMixedGender;
        IsActive = true;
        CurrentPlayersCount = 0;

        // Set max players based on sport
        MaxPlayers = GetMaxPlayersForSport(sport);
    }

    public void UpdateBasicInfo(string name, TeamColor color, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        Color = color;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignCoach(Guid? coachSubscriptionUserId)
    {
        CoachSubscriptionUserId = coachSubscriptionUserId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSeason(DateTime? season)
    {
        Season = season;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastMatchDate(DateTime matchDate)
    {
        LastMatchDate = matchDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPlayer()
    {
        if (CurrentPlayersCount >= MaxPlayers)
            throw new InvalidOperationException($"Cannot add more players. Maximum is {MaxPlayers}");

        CurrentPlayersCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePlayer()
    {
        if (CurrentPlayersCount <= 0)
            throw new InvalidOperationException("Cannot remove player. Team has no players");

        CurrentPlayersCount--;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanAddMorePlayers()
    {
        return CurrentPlayersCount < MaxPlayers;
    }

    public int AvailableSpots()
    {
        return MaxPlayers - CurrentPlayersCount;
    }

    private static int GetMaxPlayersForSport(Sport sport)
    {
        return sport switch
        {
            Sport.Football => 22,     // 11 titulares + 11 suplentes
            Sport.Basketball => 15,   // 5 titulares + 10 suplentes
            Sport.Handball => 18,     // 7 titulares + 11 suplentes
            _ => 20                   // Default
        };
    }
}