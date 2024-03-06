using System;
using System.Reflection;
using Humanizer;

namespace ToolMan.Services.CSharp.Dtos
{
    public class EntityInfoDto
    {
        public EntityInfoDto(string @namespace, string name, string? baseType, string? primaryKey, string relativeDirectory, string document)
        {
            if (relativeDirectory.Contains("Entities"))
            {
                relativeDirectory = relativeDirectory.Split("Entities")[1].RemovePreFix("/");
            }
            Namespace = @namespace;
            Name = name;
            BaseType = baseType;
            PrimaryKey = primaryKey;
            RelativeDirectory = relativeDirectory.Replace('\\', '/');
            Document = document;
        }

        public string Namespace { get; }
        public string RelativeNamespace => RelativeDirectory.Replace('/', '.');
        public string RelativeDirectory { get; }
        public string NamespaceLastPart => Namespace.Split('.').Last();
        public string Name { get; }
        public string NamePluralized => Name.Pluralize();
        public string? BaseType { get; }
        public string? PrimaryKey { get; }
        public List<PropertyInfoDto> Properties { get; } = new List<PropertyInfoDto>();
        public string? CompositeKeyName { get; set; }
        public List<PropertyInfoDto> CompositeKeys { get; } = new List<PropertyInfoDto>();
        public string? Document { get; set; }
    }

    public enum ModifierType
    {
        Public,
        Private,
        Protected,
        Internal
    }
}