using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace SchemaSharp.Tests.Utilities;

internal sealed class InMemoryAdditionalText(string path, string content) : AdditionalText
{
    private readonly SourceText _text = SourceText.From(content, Encoding.UTF8);

    public override string Path { get; } = path;

    public override SourceText? GetText(CancellationToken cancellationToken = default)
    {
        return _text;
    }
}