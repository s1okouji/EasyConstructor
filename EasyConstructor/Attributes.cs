namespace EasyConstructor;

[AttributeUsage(AttributeTargets.Class)]
public class AllArgsConstructorAttribute : Attribute
{
    public AllArgsConstructorAttribute()
    {
    }

    public AllArgsConstructorAttribute(ConstructorAccessibility accessibility)
    {
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class EmptyConstructorAttribute : Attribute
{
    public EmptyConstructorAttribute()
    {
    }

    public EmptyConstructorAttribute(ConstructorAccessibility constructorAccessibility)
    {
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RequiredArgsConstructorAttribute : Attribute
{
    public RequiredArgsConstructorAttribute()
    {
    }

    public RequiredArgsConstructorAttribute(ConstructorAccessibility constructorAccessibility)
    {
    }
}