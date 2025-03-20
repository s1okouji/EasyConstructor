using EasyConstructor;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EsyConstructorTest;

public class SyntaxExtensionTest
{
    private static object[] _isInitializedTestCase =
    [
        new Dictionary<string, bool>
        {
            {"InitializedStr", true},
            {"NotInitializedStr", false},
        }
    ];
    
    [Test, TestCaseSource(nameof(_isInitializedTestCase))]
    public void IsInitializedTest(Dictionary<string, bool> testCase)
    {
        var source =
            """
            using System;
            namespace EasyConstructorTest;
            
            public class Program
            {
                private string InitializedStr = "";
                private string NotInitializedStr;
            }
            """;
        
        var tree = CSharpSyntaxTree.ParseText(source);
        
        var variableDeclarations = tree.GetRoot().DescendantNodes().OfType<VariableDeclaratorSyntax>();
        var actual = new Dictionary<string, bool>();
        foreach (var variableDeclaratorSyntax in variableDeclarations)
        {
            actual[variableDeclaratorSyntax.Identifier.ValueText] = variableDeclaratorSyntax.IsInitialized();
        }
        Assert.That(testCase, Is.EquivalentTo(actual));
    }

    [Test]
    public void GetNonInitializedFieldIdentifiersTest()
    {
        var source =
            """
            using System;
            namespace EasyConstructorTest;

            public class Program
            {
                private string InitializedStr = "";
                private string NotInitializedStr, NotInitializedStr2;
                private string NotInitializedProperty {get; private set;}
            }
            """;
        
        var tree = CSharpSyntaxTree.ParseText(source);
        
        var classDeclaration = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        var nonInitializedVariable = classDeclaration.GetNonInitializedFieldIdentifiers();
        Assert.That(nonInitializedVariable, Is.EqualTo(new []{"NotInitializedStr","NotInitializedStr2"}));
    }
}