// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Azure.Identity;
using FalboObscura.Components;
using FalboObscura.Core.Authentication;
using FalboObscura.Core.Clients;
using FalboObscura.Core.Configuration;
using FalboObscura.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Configure Key Vault
var keyVaultUri = builder.Configuration["KeyVaultUri"];

if (!string.IsNullOrEmpty(keyVaultUri))
{
    var credential = new DefaultAzureCredential();
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), credential);
}

builder.Services.Configure<ServiceConfiguration>(builder.Configuration);
builder.Services.AddSingleton<IServiceConfiguration>(serviceProvider =>
    serviceProvider.GetRequiredService<IOptions<ServiceConfiguration>>().Value);

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=obscura.db";
builder.Services.AddDbContext<ObscuraDbContext>(options => options.UseSqlite(connectionString));

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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();

// Register Cosmos Client
builder.Services.AddSingleton<Microsoft.Azure.Cosmos.CosmosClient>(serviceProvider =>
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

builder.Services.AddSingleton<ICosmosClient, CosmosClient>();

// Register repositories
builder.Services.AddScoped<IImageRepository, ImageRepository>();

builder.Services.AddHostedService<IdentitySeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();