using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyConstructor;

public static class SyntaxExtension
{
    public static string[] GetFieldIdentifiers(this ClassDeclarationSyntax syntax)
    {
        var variables = syntax.DescendantNodes().OfType<VariableDeclaratorSyntax>();
        return variables.Select(v => v.Identifier.ValueText).ToArray();
    }
    
    public static VariableDeclaratorSyntax[] GetFieldDeclarations(this ClassDeclarationSyntax syntax)
    {
        var variables = syntax.DescendantNodes().OfType<VariableDeclaratorSyntax>();
        return variables.ToArray();
    }
    
    public static string[] GetFieldIdentifiers(this FieldDeclarationSyntax syntax)
    => syntax.Declaration.Variables.Select(variable => variable.Identifier.Text).ToArray();
    
    public static string GetPropertyIdentifiers(this PropertyDeclarationSyntax syntax)
        => syntax.Identifier.ValueText;

    /// <summary>
    /// 初期化されていないフィールド変数を取得する
    /// </summary>
    /// <param name="syntax"></param>
    /// <returns></returns>
    public static string[] GetNonInitializedFieldIdentifiers(this ClassDeclarationSyntax syntax)
    {
        return syntax.GetNonInitializedVariables().Select(v => v.Identifier.ValueText).ToArray();
    }

    public static VariableDeclaratorSyntax[] GetNonInitializedVariables(this ClassDeclarationSyntax syntax)
    {
        var fields = syntax.Members.OfType<FieldDeclarationSyntax>();
        var variables = new List<VariableDeclaratorSyntax>();
        foreach (var fieldDeclarationSyntax in fields)
        {
            variables.AddRange(fieldDeclarationSyntax.Declaration.Variables);
        }

        return variables
            .Where(v => !IsInitialized(v))
            .ToArray();
    }

    /// <summary>
    /// 変数が初期化されているかどうか
    /// </summary>
    /// <param name="syntax"></param>
    /// <returns></returns>
    public static bool IsInitialized(this VariableDeclaratorSyntax syntax)
    {
        return syntax.Initializer is not null;
    }
    
    /// <summary>
    /// 変数が初期化されているかどうか
    /// </summary>
    /// <param name="syntax"></param>
    /// <returns></returns>
    public static bool IsInitialized(this PropertyDeclarationSyntax syntax)
    {
        return syntax.Initializer is not null;
    }
}