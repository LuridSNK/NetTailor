using NetTailor.Contracts;

namespace NetTailor.Example.Contracts;

public record SampleGet(string Id) : IRequest<SampleGetResponse>;
public record SampleGetResponse(string[] Values) : IRequest<SampleGetResponse>;



public record SamplePost(string[] Items) : IRequest<SamplePostResponse>;

public record SamplePostResponse(string Result);

public record SampleDelete(string Id) : IRequest<Empty>;

public record SampleFileUpload(Stream Content) : IRequest<SampleFileUploadResponse>;
public record SampleFileUploadResponse(string CreatedId);

public record SampleFileDownload(string Id) : IRequest<Stream>;