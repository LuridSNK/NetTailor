using System.Buffers;
using System.Collections;
using System.Net.Http.Headers;

namespace NetTailor.Contracts;

public class FluentHeaderDictionary : IEnumerable<KeyValuePair<string, IEnumerable<string>>>, IDisposable
{
    private readonly List<KeyValuePair<string, IEnumerable<string>>> _headers;
    
    public FluentHeaderDictionary()
    {
        var rented = ArrayPool<KeyValuePair<string, IEnumerable<string>>>.Shared.Rent(10);
        _headers = new List<KeyValuePair<string, IEnumerable<string>>>(rented);
    }
    
    public AuthenticationHeaderValue? AuthenticationHeader { get; private set; }

    public FluentHeaderDictionary Authorization(string scheme, string value)
    {
        AuthenticationHeader = new AuthenticationHeaderValue(scheme, value);
        return this;
    }

    public FluentHeaderDictionary Add(string name, string value)
    {
        return Add(name, new[] { value });
    }

    public FluentHeaderDictionary Add(string name, string[] values)
    {
        _headers.Add(new KeyValuePair<string, IEnumerable<string>>(name, values));
        return this;
    }
    
    public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
    {
        return _headers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public void Dispose()
    {
        ArrayPool<KeyValuePair<string, IEnumerable<string>>>.Shared.Return(_headers.ToArray(), true);
    }
}