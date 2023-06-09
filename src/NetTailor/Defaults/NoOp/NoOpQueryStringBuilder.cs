﻿using NetTailor.Abstractions;

namespace NetTailor.Defaults.NoOp;

internal sealed class NoOpQueryStringBuilder<TRequest> : IQueryStringBuilder<TRequest>
{
    internal static IQueryStringBuilder<TRequest> Instance { get; } = new NoOpQueryStringBuilder<TRequest>();
    public ValueTask<string> Build(TRequest request, CancellationToken ct = default) => new(string.Empty);
}