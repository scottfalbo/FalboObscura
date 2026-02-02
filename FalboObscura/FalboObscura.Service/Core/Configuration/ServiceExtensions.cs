// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Azure.Identity;
using Azure.Storage.Blobs;
using FalboObscura.Core.Authentication;
using FalboObscura.Core.Clients;
using FalboObscura.Core.Processors;
using FalboObscura.Core.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FalboObscura.Core.Configuration;

public static class ServiceExtensions
{
    public static void AddBlobStorageClient(this WebApplicationBuilder builder, IServiceConfiguration config)
    {
        builder.Services.AddSingleton(serviceProvider =>
        {
            if (string.IsNullOrEmpty(config.BlobConnectionString))
                throw new InvalidOperationException("BlobConnectionString configuration is required");

            return new BlobServiceClient(config.BlobConnectionString);
        });

        builder.Services.AddSingleton<IBlobStorageClient, BlobStorageClient>();
    }

    public static void AddCosmosClient(this WebApplicationBuilder builder, IServiceConfiguration config)
    {
        builder.Services.AddSingleton(serviceProvider =>
        {
            if (string.IsNullOrEmpty(config.CosmosEndPoint))
                throw new InvalidOperationException("CosmosEndPoint configuration is required");

            if (string.IsNullOrEmpty(config.CosmosKey))
                throw new InvalidOperationException("CosmosKey configuration is required");

            var cosmosClientOptions = new Microsoft.Azure.Cosmos.CosmosClientOptions
            {
                SerializerOptions = new Microsoft.Azure.Cosmos.CosmosSerializationOptions
                {
                    PropertyNamingPolicy = Microsoft.Azure.Cosmos.CosmosPropertyNamingPolicy.CamelCase
                },
                ConnectionMode = Microsoft.Azure.Cosmos.ConnectionMode.Direct
            };

            return new Microsoft.Azure.Cosmos.CosmosClient(
                config.CosmosEndPoint,
                config.CosmosKey,
                cosmosClientOptions);
        });

        builder.Services.AddSingleton<ICosmosClient, CosmosClient>();
    }

    public static void AddIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<ObscuraDbContext>()
        .AddDefaultTokenProviders()
        .AddDefaultUI();
    }

    public static void AddKeyVault(this WebApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["KeyVaultUri"];

        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            try
            {
                var credential = new DefaultAzureCredential();
                builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), credential);
            }
            catch (CredentialUnavailableException ex)
            {
                // Key Vault authentication failed - continue without Key Vault for local development
                Console.WriteLine($"Warning: Unable to connect to Azure Key Vault: {ex.Message}");
                Console.WriteLine("Continuing with local configuration (appsettings.json/user secrets)");
            }
        }
    }

    public static void AddTransients(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IGalleryProcessor, GalleryProcessor>();
        builder.Services.AddTransient<IGalleryRepository, GalleryRepository>();
        builder.Services.AddTransient<IBlobStorageClient, BlobStorageClient>();
        builder.Services.AddTransient<IBlobStorageProcessor, BlobStorageProcessor>();

        // Register GameBox services
        builder.Services.AddScoped<GameBox.BlockGame.Data.GameService>();
        builder.Services.AddScoped<GameBox.PlagueDoctor.Data.PlagueService>();
    }
}