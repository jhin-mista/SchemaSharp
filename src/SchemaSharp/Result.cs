namespace SchemaSharp;

internal sealed record Result<TValue>(TValue Value, EquatableArray<DiagnosticInfo> DiagnosticInfos)
    where TValue : IEquatable<TValue>?;