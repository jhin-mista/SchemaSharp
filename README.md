# SchemaSharp

SchemaSharp is a Roslyn source generator that produces strongly-typed C# types from JSON Schema documents.

## Overview

- The generator parses JSON Schema files and produces C# POCOs using [NJsonSchema](https://github.com/RicoSuter/NJsonSchema) and its C# code generation engine.
- Generated files are emitted into the compilation as additional source files (named `<TypeName>.g.cs`).
- The generator supports custom templates using `.liquid` files. Template files are forwarded to the `NJsonSchema` engine so you can customize generated output (for example, to inject attributes or additional members).

## How it works

1. At initialization the generator registers an embedded marker attribute `GenerateFromJsonSchemaAttribute` in the `SchemaSharp` namespace.
2. During incremental execution the generator:
   - Scans the compilation for types annotated with `GenerateFromJsonSchemaAttribute`.
   - Collects all additional files whose filename ends with `schema.json` and parses them with `NJsonSchema`.
   - Collects `.liquid` template files provided as additional text and forwards their directories to the `NJsonSchema` C# generator as template directory locations.
   - For each matched schema/type the generator invokes `NJsonSchema.CodeGeneration.CSharp.CSharpGenerator` to create a C# file and adds it to the compilation as source.

The generated types use settings that favor POCO style classes, nullable reference types, and optional properties mapped to nullable C# types where appropriate.

## Usage

1. Add the generator project or package to the consuming project (as an analyzer or package).
2. Add the JSON Schema file(s) to the project as additional files. Filenames must end with `schema.json`.

Example MSBuild entry:

```xml
<ItemGroup>
  <AdditionalFiles Include="Schemas\person.schema.json" />
  <AdditionalFiles Include="Templates\Class.Property.Annotations.liquid" />
</ItemGroup>
```

3. Declare a partial type and apply the generator attribute. The type name must match the `title` in the JSON Schema so the generator can match schema -> type.

```csharp
[SchemaSharp.GenerateFromJsonSchema]
public partial class Person
{
}
```

4. If you want to customize generated code, add `.liquid` template files as additional text. Template files are forwarded to NJsonSchema and can control template rendering.

Example template usage:

- To add attributes to all properties of a type create a template file named `Class.Property.Annotations.liquid` with the desired attribute text. For example:

```liquid
[global::Microsoft.Extensions.Options.ValidateObjectMembersAttribute]
```

That content will be injected by the template into the generated property declarations according to how `NJsonSchema` templates are written.

## Notes and recommendations

- The generator matches a schema to a type by comparing the type name to the JSON Schema `title` value. Each type must match exactly one schema.
- The `.liquid` template mechanism and code-generation features are provided by [NJsonSchema](https://github.com/RicoSuter/NJsonSchema). Templates must follow `NJsonSchema` conventions for names and placements.

## Troubleshooting

- If a schema cannot be read or if multiple schemas match a type name the generator emits a diagnostic to help locate the problem.