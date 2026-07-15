using AuthService.Application.Interfaces;

namespace AuthService.Infrastructure.Security;

public class ClientRegistry : IClientRegistry
{
    private readonly HashSet<string> _validClientIds;

    public ClientRegistry(IEnumerable<string> validClientIds)
    {
        _validClientIds = new HashSet<string>(validClientIds, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsValidClient(string clientId)
    {
        return !string.IsNullOrWhiteSpace(clientId) && _validClientIds.Contains(clientId);
    }
}
