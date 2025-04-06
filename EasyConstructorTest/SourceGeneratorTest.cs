using System.Text;
using EasyConstructor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace EsyConstructorTest;

public class Tests
{
    private static IEnumerable<object[]> GetTestCase()
    {
        var emptyConstructorTestCase = GetTestCaseList("EmptyArgsConstructor");
        var allArgsConstructorTestCase = GetTestCaseList("AllArgsConstructor");
        var requiredArgsConstructorTestCase = GetTestCaseList("RequiredArgsConstructor");
        var someConstructorTestCase = GetTestCaseList("SomeConstructor");
        return emptyConstructorTestCase.Concat(allArgsConstructorTestCase).Concat(requiredArgsConstructorTestCase).Concat(someConstructorTestCase);
    }

    private static List<object[]> GetTestCaseList(string typeName)
    {
        var folderPath = Path.Combine("..", "..", "..", "TestCase", typeName);
        var i = 0;
        var list = new List<object[]>();
        while (true)
        {
            var sourceFileName = $"{typeName}TestCase_{i}.txt";
            var generatedFileName = $"{typeName}TestCase_{i}.generated.txt";
            var sourceFilePath = Path.Combine(folderPath, sourceFileName);
            var generatedFilePath = Path.Combine(folderPath, generatedFileName);
            if (File.Exists(sourceFilePath) && File.Exists(generatedFilePath))
            {
                list.Add([sourceFilePath, generatedFilePath]);
            }
            else
            {
                break;
            }
            i++;
        }
        return list;
    }
    
    [Test, TestCaseSource(nameof(GetTestCase))]
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