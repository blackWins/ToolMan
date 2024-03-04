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
            var model = JsonConvert.DeserializeObject<Dictionary<string, object>>(input.Options ?? "{}");

            if (Directory.Exists(input.TemplatePath))
            {
                await _generator.RenderAsync(input.TemplatePath, input.OutputPath, model!);
            }
            else
            {
                await _generator.RenderAsync(
                    [input.TemplatePath],
                    Path.GetDirectoryName(input.TemplatePath)!,
                    Path.Exists(input.OutputPath) ? input.OutputPath : Path.GetDirectoryName(input.OutputPath),
                    model!);
            }

            return true;
        }
    }
}
