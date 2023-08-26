using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public virtual async Task RunAsync(string templateDirectoryPath, string targetPath, object model, string searchPattern = "*.*")
        {
            DirectoryInfo dir = new DirectoryInfo(templateDirectoryPath);

            //skip contain @ symbol path
            var files = dir.GetFiles(searchPattern, SearchOption.AllDirectories)
                .Where(x => !x.FullName.Contains("@"))
                .Select(x => x.FullName)
                .ToArray();

            await RenderAsync(files, templateDirectoryPath, targetPath, model);
        }

        public virtual async Task RenderAsync(string[] files, string templateDirectoryPath, string targetPath, object model)
        {
            foreach (var file in files)
            {
                var content = await _templateRender.RenderFromFileAsync(file, model);

                if (content.IsNullOrWhiteSpace()) continue;

                var targetFileName = file
                    .Replace(templateDirectoryPath, targetPath)
                    .ToString();

                targetFileName = await _templateRender.RenderAsync(targetFileName, model);

                Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));

                await File.WriteAllTextAsync(targetFileName, content, Encoding.UTF8);
            }
        }
    }
}
