using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace ToolMan.Services.Dtos
{
    public class GenericGenerateDto
    {
        [Required]
        [DisplayName("TemplatePath")]
        [TextArea(Rows = 2)]
        public string TemplatePath { get; set; }

        [Required]
        [DisplayName("OutputPath")]
        public string OutputPath { get; set; }

        [HiddenInput]
        [DisplayName("Options")]
        public string Options { get; set; }
    }
}
