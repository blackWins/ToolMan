using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ToolMan.Services.CSharp.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace ToolMan.Services.CSharp
{
    public class CSharpAppService : ApplicationService
    {
        public async Task<EntityInfoDto> GetAbpEntityInfoAsync(string fileName)
        {
            if (fileName.Contains("*"))
            {
                fileName = Directory.GetFiles(
                        Path.GetDirectoryName(fileName)!,
                        fileName.Split("*")[1],
                        SearchOption.AllDirectories)
                    .FirstOrDefault()!;
            }

            if (!Path.Exists(fileName))
            {
                throw new UserFriendlyException(L["The file does not exist"]);
            }

            var csharpText = File.ReadAllText(fileName);

            var tree = new CSharpSyntaxTreeHelper(csharpText);

            var root = tree.GetCompilationUnitSyntax();

            BaseNamespaceDeclarationSyntax? namespaceSyntax = root
                .Descendants<NamespaceDeclarationSyntax>()
                .SingleOrDefault();

            namespaceSyntax ??= root
                .Descendants<FileScopedNamespaceDeclarationSyntax>()
                .SingleOrDefault();

            var @namespace = namespaceSyntax?.Name.ToString();

            var relativeDirectory = @namespace
                .Replace('.', '/');

            var classDeclarationSyntax = root
                .Descendants<ClassDeclarationSyntax>()
                .Single();

            var className = classDeclarationSyntax.Identifier.ToString();

            var baseList = classDeclarationSyntax.BaseList!;

            var genericNameSyntax = baseList?
                .Descendants<SimpleBaseTypeSyntax>()?
                .FirstOrDefault(node => !node.ToFullString().StartsWith("I"))? // Not interface
                .Descendants<GenericNameSyntax>()
                .FirstOrDefault();

            string baseType;

            string? primaryKey;

            IEnumerable<string>? keyNames = null;

            if (genericNameSyntax == null)
            {
                // No generic parameter -> Entity with Composite Keys
                baseType = baseList.Descendants<SimpleBaseTypeSyntax>().Single(node => !node.ToFullString().StartsWith("I")).Type.ToString();
                primaryKey = null;

                // Get composite keys
                var getKeysMethod = root.Descendants<MethodDeclarationSyntax>().Single(m => m.Identifier.ToString() == "GetKeys");
                keyNames = getKeysMethod
                    .Descendants<InitializerExpressionSyntax>()
                    .First()
                    .Descendants<IdentifierNameSyntax>()
                    .Select(id => id.Identifier.ToString());
            }
            else
            {
                // Normal entity
                baseType = genericNameSyntax.Identifier.ToString();
                primaryKey = genericNameSyntax.Descendants<TypeArgumentListSyntax>().Single().Arguments[0].ToString();
            }

            var entityDescription = root
                .Descendants<ClassDeclarationSyntax>()
                .Select(p => p.GetDocument())
                .JoinAsString(";");

            var properties = root.Descendants<PropertyDeclarationSyntax>()
                    .Select(prop => new PropertyInfoDto(prop.Type.ToString(), prop.Identifier.ToString(), prop.GetDocument()))
                    .ToList()
                ;
            var entityInfo = new EntityInfoDto(@namespace!, className, baseType, primaryKey, relativeDirectory, entityDescription);

            entityInfo.Properties.AddRange(properties);

            if (keyNames != null)
            {
                entityInfo.CompositeKeyName = $"{className}Key";
                entityInfo.CompositeKeys.AddRange(
                    keyNames.Select(k => properties.Single(prop => prop.Name == k)));
            }

            return entityInfo;
        }
    }
}
