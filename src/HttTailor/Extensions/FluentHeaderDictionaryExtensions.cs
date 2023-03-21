#nullable enable

using System.Diagnostics;
using System.Net.Http.Headers;
using HttTailor.Contracts;

namespace HttTailor.Extensions;

public static class FluentHeaderDictionaryExtensions
{
    public static void WriteHeaders(FluentHeaderDictionary source, HttpRequestHeaders target)
    {
        Debug.Assert(source is not null, $"{typeof(FluentHeaderDictionary)} is null");
        Debug.Assert(target is not null, $"{typeof(HttpRequestHeaders)} is null");
        
        if (source?.AuthenticationHeader is not null)
        {
            Debug.WriteLine($"Adding Authorization Header [{source.AuthenticationHeader.Scheme}:{source.AuthenticationHeader.Scheme}]");
            target!.Authorization = source.AuthenticationHeader;
        }
        
        foreach (var header in source!)
        {
            Debug.WriteLine($"Adding Headers [{header.Key}:{string.Join(", ", header.Value)}]");
            target!.Add(header.Key, header.Value);
        }
    } 
}