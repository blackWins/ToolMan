using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace ToolMan;

[Dependency(ReplaceServices = true)]
public class ToolManBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "ToolMan";
}
