namespace ToolMan.Services.Dtos
{
    public class DirectoryTreeDto
    {
        public DirectoryTreeDto(string filePath)
        {
            FilePath = filePath;
            Text = Path.GetFileName(filePath) ?? Path.GetDirectoryName(filePath);
            if (!Path.HasExtension(Text) || Text.StartsWith("{{") && Text.EndsWith("}}"))
            {
                Icon = "jstree-folder";
                return;
            }

            Icon = Text.StartsWith("@") ? "fa fa-cubes" : "jstree-file";
        }

        public string? Text { get; private set; }

        public string? Icon { get; private set; }

        public string FilePath { get; private set; }

        public List<DirectoryTreeDto> Children { get; private set; }

        public void AddChild(DirectoryTreeDto child)
        {
            if (Children == null)
            {
                Children = new List<DirectoryTreeDto>();
            }

            Children.Add(child);
        }
    }
}
