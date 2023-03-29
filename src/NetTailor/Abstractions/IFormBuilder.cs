using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using NetTailor.Extensions;

namespace NetTailor.Abstractions;

public interface IFormBuilder<TRequest>
{
    HttpContent Build(TRequest request);
}

public class DefaultFormBuilder<TRequest> : IFormBuilder<TRequest>
{
    private readonly Func<TRequest, object> _formBuilder;
    private readonly IContentTypeProvider _contentTypeProvider;

    public DefaultFormBuilder(Expression<Func<TRequest, object>> formBuilder, IContentTypeProvider contentTypeProvider)
    {
        _contentTypeProvider = contentTypeProvider;
        _formBuilder = formBuilder.Compile();
    }

    public HttpContent Build(TRequest request)
    {
        var obj = _formBuilder.Invoke(request);
        var accessors = PropertyAccessorFactory.GetPropertyAccessors(obj);
        var form = new MultipartFormDataContent();
        Debug.WriteLine("Building form");
        foreach (var accessor in accessors)
        {
            var value = accessor.Getter(obj);
            switch (value)
            {
                case null:
                    continue;
                case FileStream fs:
                    BuildForStream(form, fs);
                    continue;
                case IConvertible convertible:
                {
                    var stringValue = convertible.ToString(CultureInfo.InvariantCulture);
                    Debug.WriteLine($"[{GetType()}]: Packing value: {accessor.PropertyName}:{stringValue}");
                    form.Add(new StringContent(stringValue), accessor.PropertyName);
                    break;
                }
            }
        }

        return form;
    }

    private void BuildForStream(MultipartFormDataContent form, FileStream fs)
    {
        Debug.WriteLine($"[{GetType()}]: Packing file: {fs.Name}");
        var fileName = fs.Name;
        var contentType = _contentTypeProvider.TryGetContentType(fs.Name, out var type) ? type : "application/octet-stream";
        var fileContent = new StreamContent(fs);
        fileContent.Headers.ContentType.MediaType = contentType;
        form.Add(fileContent, "file", fileName);
    }
}