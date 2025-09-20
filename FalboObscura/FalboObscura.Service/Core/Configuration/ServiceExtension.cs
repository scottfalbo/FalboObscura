// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Azure.Identity;
using FalboObscura.Core.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FalboObscura.Core.Configuration;

public static class ServiceExtension
{
    public static void AddCosmosClient(this WebApplicationBuilder builder, IServiceConfiguration config)
    {
        builder.Services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IServiceConfiguration>();

            if (string.IsNullOrEmpty(configuration.CosmosEndPoint))
                throw new InvalidOperationException("CosmosEndPoint configuration is required");

            if (string.IsNullOrEmpty(configuration.CosmosKey))
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
                configuration.CosmosEndPoint,
                configuration.CosmosKey,
                cosmosClientOptions);
        });
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
            var credential = new DefaultAzureCredential();
            builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), credential);
        }
    }
}