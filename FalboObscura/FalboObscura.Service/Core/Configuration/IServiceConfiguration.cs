// ------------------------------------
// Falbo Obscura
// ------------------------------------

namespace FalboObscura.Core.Configuration;

public interface IServiceConfiguration
{
    public string AzureAdCallbackPath { get; set; }
    public string AzureAdClientId { get; set; }
    public string AzureAdInstance { get; set; }
    public string AzureAdSignedOutCallbackPath { get; set; }
    public string AzureAdTenantId { get; set; }
    public string BlobConnectionString { get; set; }
    public string BlobContainerName { get; set; }
    public string CosmosEndPoint { get; set; }
    public string CosmosKey { get; set; }
    public string KeyVaultUri { get; set; }
}