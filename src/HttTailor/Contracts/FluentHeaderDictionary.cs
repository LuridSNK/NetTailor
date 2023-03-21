using System.Collections;
using System.Net.Http.Headers;

namespace HttTailor.Contracts;

public class FluentHeaderDictionary : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
{
    private readonly Dictionary<string, IEnumerable<string>> _dictionary;
    
    public FluentHeaderDictionary()
    {
        _dictionary = new Dictionary<string, IEnumerable<string>>();
    }
    
    public AuthenticationHeaderValue? AuthenticationHeader { get; private set; }

    public FluentHeaderDictionary Authorization(string scheme, string value)
    {
        AuthenticationHeader = new AuthenticationHeaderValue(scheme, value);
        return this;
    }

    public FluentHeaderDictionary Add(string name, string? value)
    {
        _dictionary.Add(name, new []{ value });
        return this;
    }

    public FluentHeaderDictionary Add(string name, IEnumerable<string?> values)
    {
        _dictionary.Add(name, values);
        return this;
    }

    public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
    {
        return _dictionary.GetEnumerator()!;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}