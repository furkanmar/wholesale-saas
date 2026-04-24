using Microsoft.EntityFrameworkCore;
using Wholesale.Domain.Entities;

namespace Wholesale.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<User>   Users   { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
