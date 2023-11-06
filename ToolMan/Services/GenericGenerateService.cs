using Newtonsoft.Json;
using Toolkit.Generator;
using ToolMan.Services.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace ToolMan.Services
{
    public class GenericGenerateService : ApplicationService, ITransientDependency
    {
        private readonly GenericGenerator _generator;

        public GenericGenerateService(GenericGenerator generator)
        {
            _generator = generator;
        }

        public async Task<bool> RunAsync(GenericGenerateDto input)
        {
            var model = JsonConvert.DeserializeObject<Dictionary<string, object>>(input.Options) ?? new Dictionary<string, object>();

            var templatePath = input.TemplatePath.Split('.');

            if (templatePath.Length > 1 && !templatePath.Last().Contains("}}"))
            {
                await _generator.RenderAsync(new[] { input.TemplatePath }, Path.GetDirectoryName(input.TemplatePath)!, input.OutputPath, model);
            }
            else
            {
                await _generator.RenderAsync(input.TemplatePath, input.OutputPath, model);
            }

            return true;
        }
    }
}
