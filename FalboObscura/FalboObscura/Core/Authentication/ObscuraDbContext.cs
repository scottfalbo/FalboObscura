// ------------------------------------
// Falbo Obscura
// ------------------------------------

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FalboObscura.Core.Authentication;

public class ObscuraDbContext(DbContextOptions<ObscuraDbContext> options) : IdentityDbContext(options)
{
}