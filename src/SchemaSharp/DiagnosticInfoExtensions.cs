using Microsoft.CodeAnalysis;

namespace SchemaSharp;

internal static class DiagnosticInfoExtensions
{
    public static Diagnostic ToDiagnostic(this DiagnosticInfo diagnosticInfo)
    {
        return Diagnostic.Create(diagnosticInfo.Descriptor, diagnosticInfo.LocationInfo.ToLocation());
    }
}