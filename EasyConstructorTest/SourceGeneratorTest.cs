﻿using System.Text;
using EasyConstructor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace EsyConstructorTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    private static object[] _generateConstructorTestCase =
    [
        new object[]{Path.Combine("..","..","..","TestCase","EmptyArgsConstructorTestCase.txt"), Path.Combine("..","..","..","TestCase","EmptyArgsConstructorTestCase.generated.txt"),},
        new object[]{Path.Combine("..","..","..","TestCase","AllArgsConstructorTestCase.txt"), Path.Combine("..","..","..","TestCase","AllArgsConstructorTestCase.generated.txt")},
        new object[]{Path.Combine("..","..","..","TestCase","RequiredArgsConstructorTestCase.txt"), Path.Combine("..","..","..","TestCase","RequiredArgsConstructorTestCase.generated.txt")},
        new object[]{Path.Combine("..","..","..","TestCase","SomeConstructorTestCase.txt"), Path.Combine("..","..","..","TestCase","SomeConstructorTestCase.generated.txt")},
        new object[]{Path.Combine("..","..","..","TestCase","SomeConstructorTestCase_1.txt"), Path.Combine("..","..","..","TestCase","SomeConstructorTestCase_1.generated.txt")},
    ];
    
    [Test, TestCaseSource(nameof(_generateConstructorTestCase))]
    public async Task GenerateConstructorTest(string sourceFile, string generatedSourceFile)
    {
        var source = await File.ReadAllTextAsync(sourceFile);
        var genSource = await File.ReadAllTextAsync(generatedSourceFile);
        await new CSharpSourceGeneratorTest<SourceGenerator, DefaultVerifier>
        {
            TestState =
            {
                Sources = { source },
                AdditionalReferences = { MetadataReference.CreateFromFile(typeof(AllArgsConstructorAttribute).Assembly.Location) },
                GeneratedSources =
                {
                    (typeof(SourceGenerator), "SampleClass.Generated.cs", SourceText.From(genSource, Encoding.UTF8))
                }
            }
        }.RunAsync();
    }
}