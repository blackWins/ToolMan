using Microsoft.Extensions.Localization;
using ToolMan.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace ToolMan;

[Dependency(ReplaceServices = true)]
public class ToolManBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<ToolManResource> _localizer;

    public ToolManBrandingProvider(IStringLocalizer<ToolManResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}