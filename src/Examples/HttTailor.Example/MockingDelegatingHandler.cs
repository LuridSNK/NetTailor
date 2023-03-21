using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HttTailor.Extensions;

namespace HttTailor.Example;

public class MockingDelegatingHandler : DelegatingHandler
{
    private readonly object _valueToReturn;
    private readonly HttpStatusCode _statusCodeToReturn;
    private readonly Naming? _naming;

    public MockingDelegatingHandler(
        object valueToReturn, 
        HttpStatusCode statusCodeToReturn, 
        Naming? naming = null)
    {
        _valueToReturn = valueToReturn;
        _statusCodeToReturn = statusCodeToReturn;
        _naming = naming;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await Console.Out.WriteLineAsync($"PRINTING REQUEST:");
        Console.ForegroundColor = ConsoleColor.Yellow;
        await Console.Out.WriteLineAsync($"{request}");
        if (request.Content is not null)
        {
            await Console.Out.WriteLineAsync($"Content:\n{await request.Content.ReadAsStringAsync(cancellationToken)}");
        }
        Console.ResetColor();
        await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(1_800)), cancellationToken);

        //var options = JsonSerializerOptionsCache.GetSettingsOrDefault(_naming);
        var serialized = JsonSerializer.Serialize(_valueToReturn);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        return new HttpResponseMessage
        {
            Content = content,
            StatusCode = _statusCodeToReturn,
        };
    }
}