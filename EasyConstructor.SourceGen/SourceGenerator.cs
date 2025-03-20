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
                    var code = $$"""
                                 namespace {{classSymbol!.ContainingNamespace.Name}} {
                                     public partial class {{classSymbol.Name}}{
                                         public {{classSymbol.Name}}(){}
                                     }
                                 }
                                 
                                 """;
                    context.AddSource($"{classDeclarationSyntax.Identifier.Text}.Generated.cs", code);
                }else if (attributeNames.Any(name => name == "EasyConstructor.AllArgsConstructorAttribute"))
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);
                    var usings = syntaxTree.GetCompilationUnitRoot().Usings;
                    var variableDeclarations = classDeclarationSyntax.DescendantNodes().OfType<VariableDeclarationSyntax>();
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
                    context.AddSource($"{classDeclarationSyntax.Identifier.Text}.Generated.cs", sb.ToString());
                }
            }
        }
    }
}