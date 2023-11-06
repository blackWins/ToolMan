namespace ToolMan.Services.Dtos
{
    public class DirectoryTreeDto
    {
        public DirectoryTreeDto(string filePath)
        {
            FilePath = filePath;
            Text = Path.GetFileName(filePath) ?? Path.GetDirectoryName(filePath);
            Icon = Path.HasExtension(filePath) ? "jstree-file" : "jstree-folder";
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
