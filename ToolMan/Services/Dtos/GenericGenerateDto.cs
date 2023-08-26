using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace ToolMan.Services.Dtos
{
    public class GenericGenerateDto
    {
        [Required]
        public string TemplatePath { get; set; }

        [Required]
        public string OutputPath { get; set; }

        [HiddenInput]
        public string Options { get; set; }
    }
}
