using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyConstructor;

[Generator]
public class SourceGenerator: ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    { }

    public void Execute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;
        var trees = compilation.SyntaxTrees;
        foreach (var syntaxTree in trees)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();
            var classNodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var classDeclarationSyntax in classNodes)
            {
                var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);
                if (classSymbol == null) continue;
                var classGenerator = new ClassGenerator(classSymbol!.ContainingNamespace.Name, classSymbol!.DeclaredAccessibility, classSymbol.Name);
                var attributes = classSymbol.GetAttributes();
                try
                {
                    var accessibilities = GetEasyConstructorAttributes(attributes);
                    foreach (var (name, constructorAccessibility) in accessibilities)
                    {
                        if (name == "EasyConstructor.EmptyConstructorAttribute")
                        {
                            classGenerator.Constructors.Add(new ConstructorGenerator(constructorAccessibility,
                                classSymbol!.Name));
                        }

                        if (name == "EasyConstructor.AllArgsConstructorAttribute")
                        {
                            classGenerator.Constructors.Add(
                                new ConstructorGenerator(constructorAccessibility, classSymbol!.Name)
                                {
                                    Parameters = GetParameters(classDeclarationSyntax)
                                });
                        }

                        if (name == "EasyConstructor.RequiredArgsConstructorAttribute")
                        {
                            classGenerator.Constructors.Add(
                                new ConstructorGenerator(constructorAccessibility, classSymbol!.Name)
                                {
                                    Parameters = GetParameters(classDeclarationSyntax, true)
                                });
                        }
                    }


                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    if (classGenerator.Constructors.Count > 0)
                    {
                        var usings = syntaxTree.GetCompilationUnitRoot().Usings;
                        context.AddSource($"{classDeclarationSyntax.Identifier.Text}.Generated.cs",
                            CreateSource(usings, classGenerator));
                    }
                }

            }
        }
    }

    private static List<(string, ConstructorAccessibility)> GetEasyConstructorAttributes(ImmutableArray<AttributeData> attributes)
    {
        var list = new List<(string, ConstructorAccessibility)>();

        if (attributes.IsDefaultOrEmpty) return list;
        foreach (var attributeData in attributes)
        {
            var attributeClass = attributeData.AttributeClass;
            if (attributeClass?.ContainingNamespace.Name != "EasyConstructor") continue;
            var name = attributeClass.Name;
            if (name is not ("EmptyConstructorAttribute"
                or "AllArgsConstructorAttribute"
                or "RequiredArgsConstructorAttribute")) continue;

            var arguments = attributeData.GetAttributeConstructorArgument();
            if(arguments.Count == 0) list.Add(($"{attributeClass.ContainingNamespace.Name}.{name}", ConstructorAccessibility.Public));
            foreach (var typedConstant in arguments.Where(typedConstant => typedConstant.Type!.Name == "ConstructorAccessibility"))
            {
                if (typedConstant.Value is int value)
                {
                    list.Add(($"{attributeClass.ContainingNamespace.Name}.{name}", (ConstructorAccessibility)value));
                }
            }
        }

        return list;
    }
    
    private static List<(string, string)> GetParameters(SyntaxNode syntaxNode, bool requiredArgs=false)
    {
        var variableDeclarations = syntaxNode.DescendantNodes().OfType<VariableDeclarationSyntax>();
        var variables = new List<(string, string)>();
        foreach (var declaration in variableDeclarations)
        {
            // トークン化されたsyntaxは、末尾にホワイトスペースを持つのでNormalizeWhitespace()を実行する
            var type = declaration.Type.NormalizeWhitespace().ToFullString();
            foreach (var declarator in declaration.Variables)
            {
                if (requiredArgs && declarator.IsInitialized()) continue;
                var name = declarator.Identifier;
                variables.Add((type, name.ValueText));
            }
        }

        return variables;
    }

    private static string CreateSource(SyntaxList<UsingDirectiveSyntax>? usings, ClassGenerator classGenerator)
    {
        var sb = new StringBuilder();
        if (usings != null)
        {
            foreach (var usingDirectiveSyntax in usings)
            {
                sb.AppendLine(usingDirectiveSyntax.ToString());
            }

            if (usings.Value.Count > 0)
                sb.AppendLine();
        }

        sb.Append(classGenerator.Generate());
        return sb.ToString();
    }
}