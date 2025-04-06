namespace EasyConstructor;

public static class ConstructorAccessibilityExt
{
    public static string ConvertToString(this ConstructorAccessibility constructorAccessibility)
    {
        switch (constructorAccessibility)
        {
            case ConstructorAccessibility.Public:
                return "public";
            case ConstructorAccessibility.Private:
                return "private";
            case ConstructorAccessibility.Protected:
                return "protected";
            case ConstructorAccessibility.Internal:
                return "internal";
            case ConstructorAccessibility.ProtectedInternal:
                return "protected internal";
            case ConstructorAccessibility.PrivateProtected:
                return "private protected";
            default:
                throw new NotSupportedException();
        }
    }
}