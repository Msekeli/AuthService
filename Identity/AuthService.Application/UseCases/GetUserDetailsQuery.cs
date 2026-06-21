using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.UseCases;

public class GetUserDetailsQuery
{
    private readonly IUserRepository _userRepository;

    public GetUserDetailsQuery(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct)
                   ?? throw new KeyNotFoundException("User not found.");

        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }
}