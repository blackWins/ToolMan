using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ToolMan.Services.Dtos
{
    public class PreviewInputDto
    {
        [Required]
        public string Path { get; set; }

        [HiddenInput]
        public string? Options { get; set; }
    }
}
