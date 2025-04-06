using System.Text;
using Microsoft.CodeAnalysis;

namespace EasyConstructor;

public class ConstructorGenerator(ConstructorAccessibility constructorAccessibility, string className)
{
    // (Type, Name)
    public List<(string, string)> Parameters = [];

    public string Generate()
    {
        var sb = new StringBuilder();
        sb.Append($"    {constructorAccessibility.ConvertToString()} {className}(");
        var strList = new List<string>();
        var statementQueue = new Queue<string>();
        foreach (var parameter in Parameters)
        {
            // トークン化されたsyntaxは、末尾にホワイトスペースを持つのでNormalizeWhitespace()を実行する
            var type = parameter.Item1;
            var name = parameter.Item2;
            strList.Add($"{type} {name}");
            statementQueue.Enqueue($"      this.{name} = {name};");
        }
        sb.Append(string.Join(", ", strList));
        sb.AppendLine(") {");
        while (statementQueue.Count > 0)
        {
            var statement = statementQueue.Dequeue();
            sb.AppendLine(statement);
        }
        sb.AppendLine("    }");
        return sb.ToString();
    }
}

public class ClassGenerator(string ns, Accessibility accessibility, string className)
{
    public List<ConstructorGenerator> Constructors = new();
    public bool IsSealed;

    public string Generate()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace {ns} {{");
        sb.AppendLine($"  {accessibility.AccessibilityToString()} partial class {className} {{");
        foreach (var constructorGenerator in Constructors)
        {
            sb.Append(constructorGenerator.Generate());
        }
        sb.AppendLine("  }");
        sb.AppendLine("}");
        return sb.ToString();
    }
}

public static class AccessibilityHelper
{
    public static string AccessibilityToString(this Accessibility accessibility)
    {
        var classModifier = "";
        switch (accessibility)
        {
            case Accessibility.Public:
                classModifier = "public";
                break;
            case Accessibility.Protected:
                classModifier = "protected";
                break;
            case Accessibility.Private:
                classModifier = "private";
                break;
            case Accessibility.Internal:
            case Accessibility.NotApplicable:
                classModifier = "internal";
                break;
            default:
                throw new ArgumentException("This accessibility is not supported");
        }
        return classModifier;
    }
}