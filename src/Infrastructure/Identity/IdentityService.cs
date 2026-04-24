using Microsoft.EntityFrameworkCore;
using Wholesale.Application.Common.Interfaces;
using Wholesale.Infrastructure.Data;

namespace Wholesale.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly ApplicationDbContext _db;

    public IdentityService(ApplicationDbContext db) => _db = db;

    public async Task<string?> GetUsernameAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user?.Username;
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user?.Role == role;
    }
}
