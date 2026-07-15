using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string clientId, string email, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string clientId, string email, CancellationToken cancellationToken = default);
}