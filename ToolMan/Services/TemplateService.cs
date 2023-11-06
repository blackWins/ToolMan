using Newtonsoft.Json;
using Toolkit.Generator;
using ToolMan.Services.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace ToolMan.Services
{
    public class TemplateService : ApplicationService, ITransientDependency
    {
        private readonly TemplateRender _render;

        public TemplateService(TemplateRender render)
        {
            _render = render;
        }

        public async Task CreateAsync(string path)
        {
            if (Path.HasExtension(path) && !path.EndsWith("}}"))
            {
                await File.WriteAllTextAsync(path, string.Empty);
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        public async Task DeleteAsync(string path)
        {
            if (!Path.Exists(path)) return;

            if (Path.HasExtension(path) && !path.EndsWith("}}"))
            {
                File.Delete(path);
            }
            else
            {
                Directory.Delete(path, true);
            }

            await Task.CompletedTask;
        }

        public async Task RenameAsync(string path, string newName)
        {
            if (!Path.Exists(path)) return;

            var newPath = Path.Combine(Path.GetDirectoryName(path)!, newName);

            if (Path.HasExtension(path) && !path.EndsWith("}}"))
            {
                File.Move(path, newPath);
            }
            else
            {
                Directory.Move(path, newPath);
            }

            await Task.CompletedTask;
        }

        public async Task UpdateContentAsync(UpdateTemplateDto input)
        {
            if (!Path.Exists(input.Path)) return;

            await File.WriteAllTextAsync(input.Path, input.Content);

            await Task.CompletedTask;
        }

        public async Task<string> MoveAsync(string oldPath, string newPath)
        {
            if (!Path.Exists(oldPath)) return string.Empty;

            if (Path.HasExtension(newPath) && !newPath.EndsWith("}}"))
            {
                throw new UserFriendlyException("Can not move to a file path");
            }

            var fileName = Path.GetFileName(oldPath);

            newPath = Path.Combine(newPath, fileName);

            if (Path.HasExtension(oldPath) && !fileName.EndsWith("}}"))
            {
                File.Move(oldPath, newPath);
            }
            else
            {
                Directory.Move(oldPath, newPath);
            }

            return await Task.FromResult(newPath);
        }

        public async Task<DirectoryTreeDto> GetDirectoryTreeAsync(string path)
        {
            if (!Path.Exists(path)) return new DirectoryTreeDto("path error");

            var dto = BuildDirectoryFileTree(path.TrimEnd());

            return await Task.FromResult(dto);
        }

        private static DirectoryTreeDto BuildDirectoryFileTree(string directory)
        {
            var node = new DirectoryTreeDto(directory);

            foreach (var dir in Directory.GetDirectories(directory))
            {
                node.AddChild(BuildDirectoryFileTree(dir));
            }

            foreach (var file in Directory.GetFiles(directory))
            {
                node.AddChild(new DirectoryTreeDto(file));
            }

            return node;
        }

        public async Task<string> GetContentAsync(string path)
        {
            if (!Path.Exists(path)) return string.Empty;

            try
            {
                return await File.ReadAllTextAsync(path);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<string> PreviewAsync(PreviewInputDto input)
        {
            var filename = Path.GetFileName(input.Path);

            if (!Path.Exists(input.Path)) return string.Empty;

            if (!Path.HasExtension(filename)) return string.Empty;

            Dictionary<string, object>? model = input.Options.IsNullOrWhiteSpace() ? null : JsonConvert.DeserializeObject<Dictionary<string, object>>(input.Options!);

            try
            {
                return await _render.RenderFromFileAsync(input.Path, model);
            }
            catch (Exception e)
            {
                return $" Template Render Error:\r\n {e.Message}";
            }
        }
    }
}
