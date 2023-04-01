using NetTailor.Contracts;

namespace NetTailor.Example.Contracts;

public record SampleGet(string Id) : IHttpRequest<SampleGetResponse>;
public record SampleGetResponse(string[] Values) : IHttpRequest<SampleGetResponse>;



public record SamplePost(string[] Items) : IHttpRequest<SamplePostResponse>;

public record SamplePostResponse(string Result);

public record SampleDelete(string Id) : IHttpRequest<Empty>;

public record SampleFileUpload(Stream Content) : IHttpRequest<SampleFileUploadResponse>;
public record SampleFileUploadResponse(string CreatedId);

public record SampleFileDownload(string Id) : IHttpRequest<Stream>;