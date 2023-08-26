using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace ToolMan.Data;

public class ToolManDbContext : AbpDbContext<ToolManDbContext>
{
    public ToolManDbContext(DbContextOptions<ToolManDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigureSettingManagement();
        builder.ConfigureAuditLogging();

        /* Configure your own entities here */
    }
}
