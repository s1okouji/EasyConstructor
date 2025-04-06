using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyConstructor;

public static class SyntaxExtension
{
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
}