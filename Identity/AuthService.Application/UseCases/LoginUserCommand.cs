using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.UseCases;

public class LoginUserCommand
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IClientRegistry _clientRegistry;

    public LoginUserCommand(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IClientRegistry clientRegistry)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _clientRegistry = clientRegistry;
    }

    public async Task<AuthResponse> ExecuteAsync(LoginUserRequest request, CancellationToken ct = default)
    {
        if (!_clientRegistry.IsValidClient(request.ClientId))
            throw new InvalidOperationException("Unknown client.");

        var user = await _userRepository.GetByEmailAsync(request.ClientId, request.Email, ct)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse { Token = token };
    }
}