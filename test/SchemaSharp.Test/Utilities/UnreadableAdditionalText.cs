using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SchemaSharp.Tests.Utilities;

internal sealed class UnreadableAdditionalText(string path) : AdditionalText
{
    public override string Path { get; } = path;

    public override SourceText? GetText(CancellationToken cancellationToken = default)
    {
        return null;
    }
}