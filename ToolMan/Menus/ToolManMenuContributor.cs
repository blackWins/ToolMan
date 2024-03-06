using ToolMan.Localization;
using Volo.Abp.UI.Navigation;

namespace ToolMan.Menus;

public class ToolManMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<ToolManResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                ToolManMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        context.Menu.Items.Add(
           new ApplicationMenuItem(
               ToolManMenus.Generictor,
               l["Menu:Generictor"],
               "/Generictor",
               icon: "fas fa-code"
           )
       );

        context.Menu.Items.Add(
           new ApplicationMenuItem(
               ToolManMenus.CSharp,
               l["Menu:CSharp"],
               "/CSharp",
               icon: "fas fa-code"
           )
       );

        return Task.CompletedTask;
    }
}
