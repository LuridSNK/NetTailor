using NetTailor.Contracts;

namespace NetTailor.Example.Contracts;

public record SampleGet(string Id) : IHttpRequest<SampleGetResponse>;
public record SampleGetResponse(string[] Values) : IHttpRequest<SampleGetResponse>;



public record SimplePost(string[] Items) : IHttpRequest<SimplePostResponse>;

public record SimplePostResponse(string Result);

public record SimpleDelete(string Id) : IHttpRequest<Empty>;

public record FileUpload(Stream File) : IHttpRequest<FileUploadResult>;
public record FileUploadResult(string Id);


public record FileDownload(string Id) : IHttpRequest<Stream>;