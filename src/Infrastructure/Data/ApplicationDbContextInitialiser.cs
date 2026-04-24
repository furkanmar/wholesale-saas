using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wholesale.Domain.Constants;
using Wholesale.Domain.Entities;

namespace Wholesale.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
            _logger.LogInformation("Veritabanı migration tamamlandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Veritabanı başlatılırken hata oluştu.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Seed işlemi sırasında hata oluştu.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // SuperAdmin yoksa oluştur
        var superAdminExists = await _context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Role == Roles.SuperAdmin);

        if (!superAdminExists)
        {
            var superAdmin = new User
            {
                Id           = Guid.NewGuid(),
                TenantId     = Guid.Empty,
                Username     = "superadmin",
                PasswordHash = BC.HashPassword("SuperAdmin123!"),
                Role         = Roles.SuperAdmin,
                IsActive     = true,
            };

            _context.Users.Add(superAdmin);
            await _context.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("SuperAdmin kullanıcısı oluşturuldu. Şifreyi production'da değiştir!");
        }
    }
}
