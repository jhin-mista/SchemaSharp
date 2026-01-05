using Microsoft.CodeAnalysis;

namespace SchemaSharp;

internal sealed record TypeSymbolInformation(string Name, TypeKind TypeKind, string Namespace);