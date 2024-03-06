using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToolMan.Services.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace ToolMan.Pages.CSharp
{
    public class IndexModel : PageModel
    {
        [DisplayName("EntityInfo")]
        [TextArea(Rows = 2)]
        [InputInfoText("Used to quickly populate the template EntityInfo parameter")]
        [Placeholder("Fill in the file path and press Enter to confirm")]
        public string EntityInfo { get; set; }

        public GenericGenerateDto ViewModel { get; set; }

        public void OnGet()
        {
        }
    }
}
