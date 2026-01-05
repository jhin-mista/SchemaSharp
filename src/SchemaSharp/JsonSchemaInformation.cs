using NJsonSchema;

namespace SchemaSharp;

internal sealed record JsonSchemaInformation(JsonSchema JsonSchema, string FilePath);