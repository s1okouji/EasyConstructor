namespace EasyConstructor;

[AttributeUsage(AttributeTargets.Class)]
public class AllArgsConstructorAttribute: Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class EmptyConstructorAttribute: Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class RequiredArgsConstructorAttribute: Attribute;