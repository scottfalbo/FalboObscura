// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Core.Clients;

namespace FalboObscura.Core.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _containerName;
    private readonly ICosmosClient _cosmosClient;
    private readonly string _databaseName;

    public UserRepository(ICosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
        _databaseName = "FalboObscura";
        _containerName = "PageData";
    }
}