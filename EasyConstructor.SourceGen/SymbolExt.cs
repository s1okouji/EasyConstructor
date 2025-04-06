using Microsoft.CodeAnalysis;

namespace EasyConstructor;

public static class SymbolExt
{
    public static List<TypedConstant> GetAttributeConstructorArgument(this AttributeData attributeData)
    {
        return attributeData.ConstructorArguments.IsDefaultOrEmpty ? [] : attributeData.ConstructorArguments.ToList();
    }
}