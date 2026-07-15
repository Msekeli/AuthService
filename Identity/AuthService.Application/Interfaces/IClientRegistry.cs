namespace AuthService.Application.Interfaces;

public interface IClientRegistry
{
    bool IsValidClient(string clientId);
}
