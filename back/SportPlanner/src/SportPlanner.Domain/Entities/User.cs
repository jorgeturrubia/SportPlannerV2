using SportPlanner.Domain.Interfaces;
using SportPlanner.Domain.ValueObjects;

namespace SportPlanner.Domain.Entities;

public class User : Entity, IAuditable
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public string? SupabaseUserId { get; private set; } // ID del usuario en Supabase Auth
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    private User() { } // EF Constructor

    // Constructor para usuarios autenticados con Supabase (el password lo gestiona Supabase)
    public User(string firstName, string lastName, Email email, UserRole role, string? supabaseUserId = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (email is null)
            throw new ArgumentException("Email cannot be null", nameof(email));

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        SupabaseUserId = supabaseUserId;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SyncSupabaseUserId(string supabaseUserId)
    {
        if (string.IsNullOrWhiteSpace(supabaseUserId))
            throw new ArgumentException("Supabase User ID cannot be empty", nameof(supabaseUserId));

        SupabaseUserId = supabaseUserId;
        UpdatedAt = DateTime.UtcNow;
    }
}
