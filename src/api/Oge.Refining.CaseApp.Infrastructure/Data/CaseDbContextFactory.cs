using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Oge.Refining.CaseApp.Infrastructure.Data;

public sealed class CaseDbContextFactory : IDesignTimeDbContextFactory<CaseDbContext>
{
    public CaseDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CaseDbContext>()
            .UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=OgeRefiningCaseDesign;Trusted_Connection=True;",
                sql => sql.MigrationsAssembly(typeof(CaseDbContext).Assembly.FullName))
            .Options;

        return new CaseDbContext(options);
    }
}