using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SchemaSharp;

internal sealed record LocationInfo(string? FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
{
    public Location ToLocation()
    {
        return FilePath is null
            ? Location.None
            : Location.Create(FilePath, TextSpan, LineSpan);
    }

    public static LocationInfo CreateFrom(SyntaxNode node)
    {
        return CreateFrom(node.GetLocation());
    }

    public static LocationInfo CreateFrom(Location location)
    {
        var lineSpan = location.GetLineSpan();
        var filePath = location.SourceTree?.FilePath ?? lineSpan.Path;

        return new LocationInfo(filePath, location.SourceSpan, lineSpan.Span);
    }
}