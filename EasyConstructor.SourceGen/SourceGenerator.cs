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
                var attributeNames = semanticModel.GetFullAttributeName(classDeclarationSyntax);
                if (attributeNames.Any(name => name == "EasyConstructor.EmptyConstructorAttribute"))
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);
                    context.AddSource($"{classDeclarationSyntax.Identifier.Text}.Generated.cs", CreateSource(classSymbol!, null, []));
                }
                
                if (attributeNames.Any(name => name == "EasyConstructor.AllArgsConstructorAttribute"))
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);
                    var usings = syntaxTree.GetCompilationUnitRoot().Usings;
                    var variableDeclarations = classDeclarationSyntax.DescendantNodes().OfType<VariableDeclarationSyntax>();
                    var variables = new List<(string, string)>();
                    foreach (var declaration in variableDeclarations)
                    {
                        // トークン化されたsyntaxは、末尾にホワイトスペースを持つのでNormalizeWhitespace()を実行する
                        var type = declaration.Type.NormalizeWhitespace().ToFullString();
                        foreach (var declarator in declaration.Variables)
                        {
                            var name = declarator.Identifier;
                            variables.Add((type, name.ValueText));
                        }
                    }
                    context.AddSource($"{classDeclarationSyntax.Identifier.Text}.Generated.cs", CreateSource(classSymbol!, usings, variables));
                }
                
                if (attributeNames.Any(name => name == "EasyConstructor.RequiredArgsConstructorAttribute"))
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);
                    var usings = syntaxTree.GetCompilationUnitRoot().Usings;
                    var variableDeclarations = classDeclarationSyntax.DescendantNodes().OfType<VariableDeclarationSyntax>();
                    var variables = new List<(string, string)>();
                    foreach (var declaration in variableDeclarations)
                    {
                        // トークン化されたsyntaxは、末尾にホワイトスペースを持つのでNormalizeWhitespace()を実行する
                        var type = declaration.Type.NormalizeWhitespace().ToFullString();
                        foreach (var declarator in declaration.Variables)
                        {
                            if (declarator.IsInitialized()) continue;
                            var name = declarator.Identifier;
                            variables.Add((type, name.ValueText));
                        }
                    }
                    context.AddSource($"{classDeclarationSyntax.Identifier.Text}.Generated.cs", CreateSource(classSymbol!, usings, variables));
                }
            }
        }
    }

    private string CreateSource(INamedTypeSymbol classSymbol, SyntaxList<UsingDirectiveSyntax> usings, VariableDeclarationSyntax[] variableDeclarations)
    {
        var sb = new StringBuilder();
        foreach (var usingDirectiveSyntax in usings)
        {
            sb.AppendLine(usingDirectiveSyntax.ToString());
        }

        if(usings.Count > 0)
            sb.AppendLine();

        sb.AppendLine($"namespace {classSymbol!.ContainingNamespace.Name} {{");
        sb.AppendLine($"  public partial class {classSymbol.Name} {{");
        sb.Append($"    public {classSymbol.Name}(");
        var strList = new List<string>();
        var statementQueue = new Queue<string>();
        foreach (var declaration in variableDeclarations)
        {
            // トークン化されたsyntaxは、末尾にホワイトスペースを持つのでNormalizeWhitespace()を実行する
            var type = declaration.Type.NormalizeWhitespace().ToFullString();
            foreach (var declarator in declaration.Variables)
            {
                // if (declarator.IsInitialized()) continue;
                var name = declarator.Identifier;
                strList.Add($"{type} {name}");
                statementQueue.Enqueue($"      this.{name} = {name};");
            }
        }
        sb.Append(string.Join(", ", strList));
        sb.AppendLine(") {");
        while (statementQueue.Count > 0)
        {
            var statement = statementQueue.Dequeue();
            sb.AppendLine(statement);
        }
        sb.AppendLine("    }");
        sb.AppendLine("  }");
        sb.AppendLine("}");
        return sb.ToString();
    }
    
    private string CreateSource(INamedTypeSymbol classSymbol, SyntaxList<UsingDirectiveSyntax>? usings, List<(string, string)> variables)
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

        sb.AppendLine($"namespace {classSymbol!.ContainingNamespace.Name} {{");
        sb.AppendLine($"  public partial class {classSymbol.Name} {{");
        sb.Append($"    public {classSymbol.Name}(");
        var strList = new List<string>();
        var statementQueue = new Queue<string>();
        foreach (var variable in variables)
        {
            // トークン化されたsyntaxは、末尾にホワイトスペースを持つのでNormalizeWhitespace()を実行する
            var type = variable.Item1;
            var name = variable.Item2;
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
        sb.AppendLine("  }");
        sb.AppendLine("}");
        return sb.ToString();
    }
}