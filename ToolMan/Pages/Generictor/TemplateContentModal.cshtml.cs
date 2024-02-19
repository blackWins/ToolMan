using Microsoft.AspNetCore.Mvc;
using ToolMan.Services;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace ToolMan.Pages.Generictor
{
    public class TemplateContentModalModel : AbpPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string FilePath { get; set; }

        public async void OnGet()
        {
        }
    }
}
