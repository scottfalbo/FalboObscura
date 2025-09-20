// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Microsoft.AspNetCore.Identity;

namespace FalboObscura.Core.Authentication;

public class IdentitySeeder(IServiceProvider serviceProvider, IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var role = "Admin";
        if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));

        var email = configuration["AuthSeedAdminEmail"];
        var password = configuration["AuthSeedPassword"];

        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var response = await userManager.CreateAsync(user, password);
                if (response.Succeeded) await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                var inRole = await userManager.IsInRoleAsync(user, role);
                if (!inRole) await userManager.AddToRoleAsync(user, role);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}