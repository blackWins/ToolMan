using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Volo.Abp.DependencyInjection;

namespace Toolkit.Generator
{
    public class GenericGenerator : ITransientDependency
    {
        private readonly TemplateRender _templateRender;

        public GenericGenerator(TemplateRender templateRender)
        {
            _templateRender = templateRender;
        }

        public virtual async Task RenderAsync(string templateDirectoryPath, string targetPath, JObject model, string searchPattern = "*.*")
        {
            DirectoryInfo dir = new DirectoryInfo(templateDirectoryPath);

            //skip contain @ symbol path
            var files = dir.GetFiles(searchPattern, SearchOption.AllDirectories)
                .Where(x => !x.FullName.Contains(TookitConsts.PRESERVE_PATH))
                .Select(x => x.FullName)
                .ToArray();

            await RenderAsync(files, templateDirectoryPath, targetPath, model);
        }

        public virtual async Task RenderAsync(string[] files, string templateDirectoryPath, string targetPath, JObject model)
        {
            foreach (var file in files)
            {
                var targetFullFileName = await _templateRender.RenderAsync(file.Replace(templateDirectoryPath, targetPath), model);

                var targetFileName = Path.GetFileName(targetFullFileName);

                targetFullFileName = targetFullFileName.Replace("@", string.Empty);

                if (Path.GetDirectoryName(file).Contains("@") && !targetFileName.StartsWith("@") && File.Exists(targetFullFileName)) continue;

                if (targetFileName.StartsWith("$"))
                {
                    await RenderBySearchPatternAsync(
                        Path.GetDirectoryName(targetFullFileName),
                        targetFileName.Replace("$", "*"),
                        Path.GetDirectoryName(file),
                        model);

                    continue;
                }

                var content = await _templateRender.RenderFromFileAsync(file, model);

                if (content.IsNullOrWhiteSpace()) continue;

                Directory.CreateDirectory(Path.GetDirectoryName(targetFullFileName));

                await File.WriteAllTextAsync(targetFullFileName, content, Encoding.UTF8);
            }
        }

        private async Task RenderBySearchPatternAsync(string searchPath, string searchPattern, string includeFilePath, JObject model)
        {
            var targetFiles = Directory.GetFiles(searchPath, searchPattern);

            if (!targetFiles.Any()) return;

            foreach (var item in targetFiles)
            {
                var content = await _templateRender.RenderFromFileAsync(item, model, includeTemplatePath: includeFilePath);

                if (content.IsNullOrWhiteSpace()) continue;

                await File.WriteAllTextAsync(item, content, Encoding.UTF8);
            }
        }
    }
}
