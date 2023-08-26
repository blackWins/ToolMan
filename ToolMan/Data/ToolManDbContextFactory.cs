using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ToolMan.Data;

public class ToolManDbContextFactory : IDesignTimeDbContextFactory<ToolManDbContext>
{
    public ToolManDbContext CreateDbContext(string[] args)
    {

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<ToolManDbContext>()
            .UseSqlite(configuration.GetConnectionString("Default"));

        return new ToolManDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
