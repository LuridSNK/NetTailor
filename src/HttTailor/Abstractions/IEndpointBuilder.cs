﻿namespace HttTailor.Abstractions;

public interface IEndpointBuilder<in TRequest>
{
    ValueTask<string> Build(TRequest request);
}