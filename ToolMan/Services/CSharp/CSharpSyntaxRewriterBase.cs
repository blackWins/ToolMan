using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ToolMan.Services.CSharp
{
    public class CSharpSyntaxRewriterBase : CSharpSyntaxRewriter
    {

        protected SyntaxTokenList CreateTokenList(SyntaxKind[] kinds)
        {
            var tokens = new List<SyntaxToken>();
            foreach (SyntaxKind kind in kinds)
            {
                tokens.Add(CreateToken(kind));
            }

            return SyntaxFactory.TokenList(tokens);
        }

        protected SyntaxToken CreateToken(SyntaxKind kind, bool startWithSpaceTrivia = true, bool endWithLineFeedTrivia = false)
        {
            return SyntaxFactory.Token(
                    startWithSpaceTrivia ? SyntaxFactory.TriviaList(SyntaxFactory.Space) : SyntaxFactory.TriviaList(),
                    kind,
                    SyntaxFactory.TriviaList(endWithLineFeedTrivia ? SyntaxFactory.LineFeed : SyntaxFactory.Space)
                   );
        }


        protected static AccessorListSyntax PublicAccessorList = SyntaxFactory.AccessorList(
                 SyntaxFactory.List(new AccessorDeclarationSyntax[]
                 {
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                 }));

        protected static SyntaxTokenList PublicModifierType = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

        protected static SyntaxTriviaList NewLine = SyntaxTriviaList.Create(SyntaxFactory.CarriageReturnLineFeed);
    }
}
