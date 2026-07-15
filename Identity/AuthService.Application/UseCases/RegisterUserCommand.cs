using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.UseCases;

public class RegisterUserCommand
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IClientRegistry _clientRegistry;

    public RegisterUserCommand(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IClientRegistry clientRegistry)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _clientRegistry = clientRegistry;
    }

    public async Task ExecuteAsync(RegisterUserRequest request, CancellationToken ct = default)
    {
        if (!_clientRegistry.IsValidClient(request.ClientId))
            throw new InvalidOperationException("Unknown client.");

        if (await _userRepository.ExistsByEmailAsync(request.ClientId, request.Email, ct))
            throw new InvalidOperationException("Email already exists.");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(
            request.ClientId,
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHash);

        await _userRepository.AddAsync(user, ct);
    }
}