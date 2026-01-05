using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SchemaSharp.Tests;

public sealed class DiagnosticInfoExtensionsTests
{
    [Test]
    public void ToDiagnostic_WhenCalled_ShouldCreateDiagnosticFromDescriptorAndLocationInfo()
    {
        // arrange
        var descriptor = CreateDescriptor();
        const string filePath = "diagnostic.cs";
        var textSpan = new TextSpan(3, 7);
        var lineSpan = new LinePositionSpan(new LinePosition(1, 3), new LinePosition(1, 10));
        var location = Location.Create(filePath, textSpan, lineSpan);
        var diagnosticInfo = new DiagnosticInfo(descriptor, location);

        // act
        var diagnostic = diagnosticInfo.ToDiagnostic();

        // assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(diagnostic.Descriptor, Is.EqualTo(descriptor));
            Assert.That(diagnostic.Location.GetLineSpan().Path, Is.EqualTo(filePath));
            Assert.That(diagnostic.Location.SourceSpan, Is.EqualTo(textSpan));
            Assert.That(diagnostic.Location.GetLineSpan().Span, Is.EqualTo(lineSpan));
        }
    }

    private static DiagnosticDescriptor CreateDescriptor()
    {
        return new DiagnosticDescriptor(
            id: "VN0001",
            title: "Title",
            messageFormat: "Message",
            category: "Category",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
    }
}