using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SchemaSharp.Tests;

public sealed class LocationInfoTests
{
    [Test]
    public void ToLocation_WhenFilePathIsNull_ShouldReturnLocationNone()
    {
        // arrange
        var info = new LocationInfo(null, default, default);

        // act
        var location = info.ToLocation();

        // assert
        Assert.That(location, Is.EqualTo(Location.None));
    }

    [Test]
    public void ToLocation_WhenFilePathProvided_ShouldGenerateLocationUsingMetadata()
    {
        // arrange
        const string filePath = "sample.cs";
        var textSpan = new TextSpan(1, 5);
        var lineSpan = new LinePositionSpan(new LinePosition(0, 1), new LinePosition(0, 6));
        var info = new LocationInfo(filePath, textSpan, lineSpan);

        // act
        var location = info.ToLocation();

        // assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(location.SourceSpan, Is.EqualTo(textSpan));
            Assert.That(location.GetLineSpan().Span, Is.EqualTo(lineSpan));
            Assert.That(location.GetLineSpan().Path, Is.EqualTo(filePath));
        }
    }

    [Test]
    public void CreateFrom_WhenSyntaxNodeProvided_ShouldCaptureNodeLocation()
    {
        // arrange
        const string source = "class Sample { void Execute() { } }";
        const string filePath = "SyntaxNodeFile.cs";
        var tree = CSharpSyntaxTree.ParseText(source, path: filePath);
        var node = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        var expectedSpan = node.GetLocation().GetLineSpan().Span;

        // act
        var info = LocationInfo.CreateFrom(node);

        // assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.FilePath, Is.EqualTo(filePath));
            Assert.That(info.TextSpan, Is.EqualTo(node.Span));
            Assert.That(info.LineSpan, Is.EqualTo(expectedSpan));
        }
    }

    [Test]
    public void CreateFrom_WhenLocationProvidedWithFilePathInLineSpan_ShouldCaptureLocationMetadata()
    {
        // arrange
        const string filePath = "location.cs";
        var textSpan = new TextSpan(2, 4);
        var lineSpan = new LinePositionSpan(new LinePosition(1, 0), new LinePosition(1, 4));
        var location = Location.Create(filePath, textSpan, lineSpan);

        // act
        var info = LocationInfo.CreateFrom(location);

        // assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.FilePath, Is.EqualTo(filePath));
            Assert.That(info.TextSpan, Is.EqualTo(textSpan));
            Assert.That(info.LineSpan, Is.EqualTo(lineSpan));
        }
    }

    [Test]
    public void CreateFromWhenLocationProvidedWithFilePathInSyntaxTree_ShouldCaptureLocationMetadata()
    {
        // arrange
        const string source = "class Sample { void Execute() { } }";
        const string filePath = "LocationSyntaxTree.cs";
        var tree = CSharpSyntaxTree.ParseText(source, path: filePath);
        var node = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First();
        var location = node.GetLocation();
        var expectedSpan = location.GetLineSpan().Span;

        // act
        var info = LocationInfo.CreateFrom(location);

        // assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.FilePath, Is.EqualTo(filePath));
            Assert.That(info.TextSpan, Is.EqualTo(location.SourceSpan));
            Assert.That(info.LineSpan, Is.EqualTo(expectedSpan));
        }
    }
}