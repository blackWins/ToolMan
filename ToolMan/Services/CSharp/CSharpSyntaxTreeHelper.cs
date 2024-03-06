using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using ToolMan.Services.CSharp.Dtos;

namespace ToolMan.Services.CSharp
{
    public class CSharpSyntaxTreeHelper
    {
        private SyntaxTree Tree { get; set; }

        private string FullName { get; }

        public CSharpSyntaxTreeHelper() { }

        /// <param name="input"></param>
        /// <param name="isInputPath">input is physical path</param>
        public CSharpSyntaxTreeHelper(string input)
        {
            if (Directory.Exists(input))
            {
                FullName = input;

                var content = File.ReadAllText(input);

                Tree = CSharpSyntaxTree.ParseText(content);
            }
            else
            {
                Tree = CSharpSyntaxTree.ParseText(input);
            }
        }

        public override string ToString()
        {
            return Tree.ToString();
        }

        public CompilationUnitSyntax GetCompilationUnitSyntax()
        {
            return Tree.GetCompilationUnitRoot();
        }

        public IEnumerable<SyntaxNode> DescendantNodes()
        {
            return GetCompilationUnitSyntax().DescendantNodes();
        }

        public PropertyDeclarationSyntax[] GetPropertyDeclaration(ModifierType? modifier = ModifierType.Public)
        {
            var modifierType = modifier.ToString().ToCamelCase();

            return DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .WhereIf(modifier != null, w => w.Modifiers.First().Value!.Equals(modifierType))
                .ToArray();
        }

        public string GetDescendantNodesChildText<TSyntaxNode>(Func<TSyntaxNode, bool> predicate) where TSyntaxNode : SyntaxNode
        {
            var nodes = GetCompilationUnitSyntax().DescendantNodes().OfType<TSyntaxNode>().Where(predicate).ToList();

            StringBuilder stringBuilder = new StringBuilder();

            nodes.ForEach(n =>
            {
                foreach (var node in n.DescendantNodes())
                {
                    stringBuilder.Append(node.ToString());
                }
            });

            return stringBuilder.ToString();
        }

        public void Rewriter<TWriter>(TWriter visitor) where TWriter : CSharpSyntaxRewriterBase
        {
            var result = visitor.Visit(GetCompilationUnitSyntax());

            Tree = CSharpSyntaxTree.ParseText(result.GetText().ToString());
        }

        public QualifiedNameSyntax[] GetNameSpace()
        {
            return DescendantNodes().OfType<QualifiedNameSyntax>().ToArray();
        }

        public bool SaveChanges(string? path = null)
        {
            if (path == null && FullName.IsNullOrWhiteSpace())
            {
                return false;
            }

            path = path ?? FullName;

            if (Path.HasExtension(path))
            {
                path = Path.GetDirectoryName(path);
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var filePath = Path.Combine(path, DescendantNodes().OfType<ClassDeclarationSyntax>().First().Identifier.ToString()) + ".cs";

            File.WriteAllText(filePath, Tree.ToString());

            return true;
        }
    }
}