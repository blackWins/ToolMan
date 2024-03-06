namespace ToolMan.Services.CSharp.Dtos
{
    public class PropertyInfoDto
    {
        public string Type { get; }

        public string Name { get; }

        public string Document { get; set; }

        public PropertyInfoDto(string type, string name, string document)
        {
            Type = type;
            Name = name;
            Document = document;
        }
    }
}