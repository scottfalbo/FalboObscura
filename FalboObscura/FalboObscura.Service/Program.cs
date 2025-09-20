// ------------------------------------
// Falbo Obscura
// ------------------------------------

using FalboObscura.Components;
using FalboObscura.Core.Authentication;
using FalboObscura.Core.Clients;
using FalboObscura.Core.Configuration;
using FalboObscura.Core.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddKeyVault();

builder.Services.Configure<ServiceConfiguration>(builder.Configuration);
var serviceConfig = new ServiceConfiguration();

builder.Configuration.Bind(serviceConfig);
builder.Services.AddSingleton<IServiceConfiguration>(serviceConfig);

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=obscura.db";
builder.Services.AddDbContext<ObscuraDbContext>(options => options.UseSqlite(connectionString));

builder.AddIdentity();

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
builder.AddCosmosClient(serviceConfig);

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