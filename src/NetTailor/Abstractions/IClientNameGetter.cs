﻿namespace NetTailor.Abstractions;

/// <summary>
/// Gets the name of a profile of <see cref="HttpClient"/>.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IClientNameGetter<TRequest>
{
    /// <summary>
    /// The <see cref="HttpClient"/> name/
    /// </summary>
    public string Name { get; }
}