using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Symposium.Storage;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("database");
        if (string.IsNullOrWhiteSpace(connection))
            connection = "Host=127.0.0.1;Port=1;Database=symposium_design;Username=design;Timeout=1";

        return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connection, postgres => postgres.SetPostgresVersion(18, 1)
                .MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name))
            .Options);
    }
}
