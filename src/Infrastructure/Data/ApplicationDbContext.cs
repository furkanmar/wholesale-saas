using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Wholesale.Application.Common.Interfaces;
using Wholesale.Domain.Entities;

namespace Wholesale.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ITenantContext _tenantContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User>   Users   => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global query filter — SuperAdmin tüm tenant'ları görebilir
        builder.Entity<User>()
            .HasQueryFilter(u =>
                _tenantContext.IsSuperAdmin ||
                u.TenantId == _tenantContext.TenantId);
    }
}
