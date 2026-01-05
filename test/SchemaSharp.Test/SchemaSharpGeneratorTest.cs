using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SchemaSharp.Tests.Utilities;
using System.Collections.Immutable;

namespace SchemaSharp.Tests;

public sealed class SchemaSharpGeneratorTest
{
    [Test]
    public void GenerateType_WhenSchemaMissing_ShouldReportDiagnostic()
    {
        // arrange
        const string source = """
using SchemaSharp;

[GenerateFromJsonSchema]
public sealed partial class SampleConfig
{
}
""";
        var compilation = CreateCompilation(source);
        var generatorDriver = CreateGeneratorDriver();

        // act
        generatorDriver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out var diagnostics);

        // assert
        var allDiagnosticMessages = GetAllDiagnosticMessages(diagnostics);
        Assert.That(diagnostics, Has.Length.EqualTo(1).And.One.Matches<Diagnostic>(x => x.Id == "SCHEMASHARP0003"), allDiagnosticMessages);
    }

    [Test]
    public void GenerateType_WhenSchemaFileAvailable_ShouldGenerateSources()
    {
        // arrange
        const string source = """
using SchemaSharp;

[GenerateFromJsonSchema]
public sealed partial class SampleConfig
{
}
""";

        const string schemaJson = """
{
  "title": "SampleConfig",
  "type": "object",
  "additionalProperties": false,
  "properties": {
    "name": { "type": "string" }
  }
}
""";

        var compilation = CreateCompilation(source);
        var inMemoryAdditionalText = new InMemoryAdditionalText("SampleConfig.schema.json", schemaJson);
        AdditionalText[] additionalTexts = [inMemoryAdditionalText];

        var generatorDriver = CreateGeneratorDriver().AddAdditionalTexts(additionalTexts.ToImmutableArray());

        // act
        var resultGeneratorDriver = generatorDriver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out var diagnostics);

        // assert
        Assert.That(diagnostics, Is.Empty, GetAllDiagnosticMessages(diagnostics));

        var runResult = resultGeneratorDriver.GetRunResult();
        var generatedSources = runResult.Results.SelectMany(r => r.GeneratedSources).ToArray();
        Assert.That(generatedSources, Has.Length.EqualTo(3));

        Assert.That(generatedSources, Has.One.Matches<GeneratedSourceResult>(x => x.HintName == "Microsoft.CodeAnalysis.EmbeddedAttribute.cs"));
        Assert.That(generatedSources, Has.One.Matches<GeneratedSourceResult>(x => x.HintName == "GenerateJsonSchemaAttribute.g.cs"));

        var schemaClassSource = generatedSources.Single(x => x.HintName == "SampleConfig.g.cs");

        using (Assert.EnterMultipleScope())
        {
            var generatedText = schemaClassSource.SourceText.ToString();
            Assert.That(generatedText, Does.Contain("public partial class SampleConfig"));
            Assert.That(generatedText, Does.Contain("public string? Name { get; set; }"));
        }
    }

    [Test]
    public void GenerateType_WhenSchemaTitleMatchesMultipleSchemas_ShouldReportDiagnostic()
    {
        // arrange
        const string source = """
using SchemaSharp;

[GenerateFromJsonSchema]
public sealed partial class SampleConfig
{
}
""";

        const string schemaJson1 = """
{ "title": "SampleConfig", "type": "object" }
""";

        const string schemaJson2 = """
{ "title": "SampleConfig", "type": "object" }
""";

        var compilation = CreateCompilation(source);
        AdditionalText[] additionalTexts =
        [
            new InMemoryAdditionalText("a/SampleConfig.schema.json", schemaJson1),
            new InMemoryAdditionalText("b/SampleConfig.schema.json", schemaJson2),
        ];

        var generatorDriver = CreateGeneratorDriver().AddAdditionalTexts(additionalTexts.ToImmutableArray());

        // act
        generatorDriver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out var diagnostics);

        // assert
        var allDiagnosticMessages = GetAllDiagnosticMessages(diagnostics);
        Assert.That(diagnostics, Has.Length.EqualTo(1).And.One.Matches<Diagnostic>(x => x.Id == "SCHEMASHARP0003"), allDiagnosticMessages);
    }

    [Test]
    public void GenerateType_WhenSchemaCannotBeRead_ShouldReportDiagnostic()
    {
        // arrange
        const string source = """
using SchemaSharp;

[GenerateFromJsonSchema]
public sealed partial class SampleConfig
{
}
""";

        var compilation = CreateCompilation(source);
        AdditionalText[] additionalTexts =
        [
            new UnreadableAdditionalText("SampleConfig.schema.json"),
        ];

        var generatorDriver = CreateGeneratorDriver().AddAdditionalTexts(additionalTexts.ToImmutableArray());

        // act
        generatorDriver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out var diagnostics);

        // assert
        var allDiagnosticMessages = GetAllDiagnosticMessages(diagnostics);
        Assert.That(diagnostics, Has.Length.AtLeast(1).And.One.Matches<Diagnostic>(x => x.Id == "SCHEMASHARP0002"), allDiagnosticMessages);
    }

    [Test]
    public void GenerateType_WhenLiquidTemplatesPresent_ShouldNotThrowAndShouldGenerate()
    {
        // arrange
        const string source = """
using SchemaSharp;

[GenerateFromJsonSchema]
public sealed partial class SampleConfig
{
}
""";

        const string schemaJson = """
{
  "title": "SampleConfig",
  "type": "object",
  "additionalProperties": false,
  "properties": {
    "name": { "type": "string" }
  }
}
""";

        var compilation = CreateCompilation(source);
        const string Path = "../../../TestFiles/Class.Property.Annotations.liquid";

        AdditionalText[] additionalTexts =
        [
            new InMemoryAdditionalText("SampleConfig.schema.json", schemaJson),
            new FileAdditionalText(Path),
        ];

        var expectedAnnotation = File.ReadAllText(Path);

        var generatorDriver = CreateGeneratorDriver().AddAdditionalTexts(additionalTexts.ToImmutableArray());

        // act
        GeneratorDriver? resultGeneratorDriver = null;
        Assert.DoesNotThrow(() =>
            resultGeneratorDriver = generatorDriver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _));

        // assert
        var runResult = resultGeneratorDriver!.GetRunResult();
        var generatedSourceResults = runResult.Results.SelectMany(r => r.GeneratedSources);
        var allDiagnosticMessages = GetAllDiagnosticMessages(runResult.Diagnostics);

        Assert.That(allDiagnosticMessages, Is.Empty);
        var generatedSource = generatedSourceResults.Single(x => x.HintName == "SampleConfig.g.cs");

        Assert.That(generatedSource.SourceText.ToString(), Does.Contain(expectedAnnotation));
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var references = GetMetadataReferences().ToArray();
        var assemblyFileName = typeof(SchemaSharpGeneratorTest).Assembly.GetName().Name;
        var assemblyName = Path.GetFileNameWithoutExtension(assemblyFileName);
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        IEnumerable<SyntaxTree> syntaxTrees = [CSharpSyntaxTree.ParseText(source)];

        return CSharpCompilation.Create(assemblyName, syntaxTrees, references, compilationOptions);
    }

    private static CSharpGeneratorDriver CreateGeneratorDriver()
    {
        var schemaSharpGenerator = new SchemaSharpGenerator();

        return CSharpGeneratorDriver.Create(schemaSharpGenerator);
    }

    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location))
            {
                continue;
            }

            yield return MetadataReference.CreateFromFile(assembly.Location);
        }
    }

    private static string GetAllDiagnosticMessages(ImmutableArray<Diagnostic> diagnostics)
    {
        return string.Join(Environment.NewLine, diagnostics.Select(x => x.GetMessage()));
    }
}