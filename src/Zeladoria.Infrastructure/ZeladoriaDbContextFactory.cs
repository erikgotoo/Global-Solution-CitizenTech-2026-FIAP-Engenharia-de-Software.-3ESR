using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zeladoria.Infrastructure;

/// <summary>Usada apenas pelas ferramentas do EF Core (dotnet ef migrations add / database update).</summary>
public class ZeladoriaDbContextFactory : IDesignTimeDbContextFactory<ZeladoriaDbContext>
{
    public ZeladoriaDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ZeladoriaDbContext>()
            .UseSqlite("Data Source=zeladoria.db")
            .Options;
        return new ZeladoriaDbContext(options);
    }
}
