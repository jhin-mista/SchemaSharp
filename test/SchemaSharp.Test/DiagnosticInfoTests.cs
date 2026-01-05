using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SchemaSharp.Tests;

public sealed class DiagnosticInfoTests
{
    [Test]
    public void DiagnosticInfo_WhenLocationIsNull_ShouldUseLocationNone()
    {
        // arrange
        var descriptor = CreateDescriptor();

        // act
        var info = new DiagnosticInfo(descriptor, location: null);

        // assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Descriptor, Is.SameAs(descriptor));
            Assert.That(info.LocationInfo, Is.EqualTo(LocationInfo.CreateFrom(Location.None)));
        }
    }

    [Test]
    public void DiagnosticInfo_WhenLocationProvided_ShouldCreateLocationInfoFromLocation()
    {
        // arrange
        var descriptor = CreateDescriptor();
        const string filePath = "diagnostic.cs";
        var textSpan = new TextSpan(3, 7);
        var lineSpan = new LinePositionSpan(new LinePosition(1, 3), new LinePosition(1, 10));
        var location = Location.Create(filePath, textSpan, lineSpan);

        // act
        var info = new DiagnosticInfo(descriptor, location);

        // assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Descriptor, Is.SameAs(descriptor));
            Assert.That(info.LocationInfo, Is.EqualTo(LocationInfo.CreateFrom(location)));
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