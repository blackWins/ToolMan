using Microsoft.AspNetCore.Mvc.RazorPages;
using ToolMan.Services.Dtos;

namespace AbpHelper.Pages.Generictor
{
    public class IndexModel : PageModel
    {
        public GenericGenerateDto ViewModel { get; set; }

        public void OnGet()
        {
        }
    }
}
