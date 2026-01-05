using Microsoft.CodeAnalysis;

namespace SchemaSharp;

internal sealed record DiagnosticInfo
{
    public DiagnosticInfo(DiagnosticDescriptor descriptor, Location? location)
    {
        Descriptor = descriptor;
        LocationInfo = LocationInfo.CreateFrom(location ?? Location.None);
    }

    public DiagnosticDescriptor Descriptor { get; }
    public LocationInfo LocationInfo { get; }
}