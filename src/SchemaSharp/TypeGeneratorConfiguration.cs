using NJsonSchema;

namespace SchemaSharp;

internal sealed record TypeGeneratorConfiguration(TypeSymbolInformation TypeSymbolInformation, JsonSchema JsonSchema);