using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace SchemaSharp.Tests.Utilities;

internal sealed class FileAdditionalText(string path) : AdditionalText
{
    public override string Path => path;

    public override SourceText? GetText(CancellationToken cancellationToken = default)
    {
        var fileText = File.ReadAllText(Path);
        return SourceText.From(fileText, Encoding.UTF8);
    }
}
