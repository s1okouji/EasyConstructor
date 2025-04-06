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

    public static AttributeSyntax[] GetAttributes(this SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        return (from attributeListSyntax in classDeclarationSyntax.AttributeLists
            from attributeSyntax in attributeListSyntax.Attributes
            select attributeSyntax).ToArray();
    }

    public static List<TypedConstant> GetAttributeConstructorArgument(this AttributeData attributeData)
    {
        return attributeData.ConstructorArguments.IsDefaultOrEmpty ? [] : attributeData.ConstructorArguments.ToList();
    }
}