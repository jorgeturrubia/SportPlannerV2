using SportPlanner.Domain.Enum;

namespace SportPlanner.Domain.Entities.Planning;

public class Exercise
{
    public Guid Id { get; private set; }
    public Guid? SubscriptionId { get; private set; }
    public ContentOwnership Ownership { get; private set; }
    public Guid? MarketplaceUserId { get; private set; }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid TypeId { get; private set; }

    public string? VideoUrl { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? Instructions { get; private set; }

    public int? DefaultSets { get; private set; }
    public int? DefaultReps { get; private set; }
    public int? DefaultDurationSeconds { get; private set; }
    public string? DefaultIntensity { get; private set; }

    public bool IsActive { get; private set; }


    // Audit
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    private Exercise() { }

    public Exercise(
        Guid? subscriptionId,
        ContentOwnership ownership,
        string name,
        string description,
        Guid categoryId,
        Guid typeId,
        string createdBy,
        Guid? marketplaceUserId = null)
    {
        ValidateOwnership(subscriptionId, ownership, marketplaceUserId);

        Id = Guid.NewGuid();
        SubscriptionId = subscriptionId;
        Ownership = ownership;
        MarketplaceUserId = marketplaceUserId;
        Name = name;
        Description = description;
        CategoryId = categoryId;
        TypeId = typeId;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void Update(
        string name,
        string description,
        Guid categoryId,
        Guid typeId,
        string updatedBy)
    {
        if (Ownership == ContentOwnership.System)
            throw new InvalidOperationException("Cannot modify system content directly. Clone it first.");

        Name = name;
        Description = description;
        CategoryId = categoryId;
        TypeId = typeId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetMediaUrls(string? videoUrl, string? imageUrl, string updatedBy)
    {
        if (Ownership == ContentOwnership.System)
            throw new InvalidOperationException("Cannot modify system content directly. Clone it first.");

        VideoUrl = videoUrl;
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetInstructions(string instructions, string updatedBy)
    {
        if (Ownership == ContentOwnership.System)
            throw new InvalidOperationException("Cannot modify system content directly. Clone it first.");

        Instructions = instructions;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetDefaults(
        int? sets,
        int? reps,
        int? durationSeconds,
        string? intensity,
        string updatedBy)
    {
        if (Ownership == ContentOwnership.System)
            throw new InvalidOperationException("Cannot modify system content directly. Clone it first.");

        if (sets.HasValue && sets.Value <= 0)
            throw new ArgumentException("Sets must be greater than 0");
        if (reps.HasValue && reps.Value <= 0)
            throw new ArgumentException("Reps must be greater than 0");
        if (durationSeconds.HasValue && durationSeconds.Value <= 0)
            throw new ArgumentException("Duration must be greater than 0");

        DefaultSets = sets;
        DefaultReps = reps;
        DefaultDurationSeconds = durationSeconds;
        DefaultIntensity = intensity;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Activate(string updatedBy)
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public Exercise Clone(Guid targetSubscriptionId, string createdBy)
    {
        if (Ownership == ContentOwnership.User)
            throw new InvalidOperationException("Cannot clone user content directly.");

        var cloned = new Exercise(
            targetSubscriptionId,
            ContentOwnership.User,
            Name,
            Description,
            CategoryId,
            TypeId,
            createdBy);

        cloned.VideoUrl = VideoUrl;
        cloned.ImageUrl = ImageUrl;
        cloned.Instructions = Instructions;
        cloned.DefaultSets = DefaultSets;
        cloned.DefaultReps = DefaultReps;
        cloned.DefaultDurationSeconds = DefaultDurationSeconds;
        cloned.DefaultIntensity = DefaultIntensity;

        return cloned;
    }

    private static void ValidateOwnership(Guid? subscriptionId, ContentOwnership ownership, Guid? marketplaceUserId)
    {
        if (ownership == ContentOwnership.System && subscriptionId.HasValue)
            throw new ArgumentException("System content cannot have SubscriptionId");

        if (ownership == ContentOwnership.User && !subscriptionId.HasValue)
            throw new ArgumentException("User content must have SubscriptionId");

        if (ownership == ContentOwnership.MarketplaceUser && !marketplaceUserId.HasValue)
            throw new ArgumentException("Marketplace content must have MarketplaceUserId");
    }
}