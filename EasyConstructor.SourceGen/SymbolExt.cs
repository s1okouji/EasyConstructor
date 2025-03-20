using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyConstructor;

public static class SymbolExt
{
    public static string[] GetFullAttributeName(this SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        return (from attributeListSyntax in classDeclarationSyntax.AttributeLists
                from attributeSyntax in attributeListSyntax.Attributes
                // ReSharper disable once RedundantEnumerableCastCall
                select semanticModel.GetSymbolInfo(attributeSyntax).Symbol?.ContainingType).OfType<INamedTypeSymbol>()
            .Select(attributeType => $"{attributeType.ContainingNamespace.Name}.{attributeType.Name}").ToArray();
    }
}