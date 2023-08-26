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
            var model = JsonConvert.DeserializeObject<Dictionary<string, object>>(input.Options);

            await _generator.RunAsync(input.TemplatePath, input.OutputPath, model);

            return true;
        }
    }
}
