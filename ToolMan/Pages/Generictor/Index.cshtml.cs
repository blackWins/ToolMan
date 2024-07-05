using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToolMan.Services.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace AbpHelper.Pages.Generictor
{
    public class IndexModel : PageModel
    {
        [DisplayName("AbpEntityInfo")]
        [TextArea(Rows = 2)]
        [InputInfoText("Used to quickly populate the template EntityInfo parameter")]
        [Placeholder("Fill in the file path and press Enter to confirm")]
        public string EntityInfo { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IsAbp { get; set; }

        public GenericGenerateDto ViewModel { get; set; }

        public void OnGet()
        {
        }
    }
}
