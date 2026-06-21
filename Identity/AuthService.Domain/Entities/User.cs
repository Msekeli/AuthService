namespace AuthService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    // Constructor used when creating a new user in the application
    public User(string firstName, string lastName, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;

        FirstName = !string.IsNullOrWhiteSpace(firstName)
            ? firstName
            : throw new ArgumentException("First name is required.", nameof(firstName));

        LastName = !string.IsNullOrWhiteSpace(lastName)
            ? lastName
            : throw new ArgumentException("Last name is required.", nameof(lastName));

        Email = !string.IsNullOrWhiteSpace(email)
            ? email
            : throw new ArgumentException("Email is required.", nameof(email));

        PasswordHash = !string.IsNullOrWhiteSpace(passwordHash)
            ? passwordHash
            : throw new ArgumentException("Password hash is required.", nameof(passwordHash));
    }

    // Constructor for EF Core
    protected User()
    {
       
    }
}