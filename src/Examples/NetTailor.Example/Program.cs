using Microsoft.Extensions.DependencyInjection;
using NetTailor;
using NetTailor.Contracts;
using NetTailor.Example;
using NetTailor.Example.Contracts;
using NetTailor.Extensions;
using NetTailor.Utilities;

// define service collection & configuration
var serviceProvider = new ServiceCollection()
    .AddSingleton(Configuration.Value)
    .AddHttpClientProfile<ExampleClientProfile>() // add profile
    .BuildServiceProvider();

var dispatcher = serviceProvider.GetRequiredService<IRequestDispatcher>();

var postResult = await dispatcher.Dispatch<SamplePost, SamplePostResponse>(
    new SamplePost(Enumerable.Range(1, 5).Select(_ => RandomNames.Generate()).ToArray()), 
    CancellationToken.None);
postResult.WriteResult<SamplePost, SamplePostResponse>();

var getResult = await dispatcher.Dispatch<SampleGet, SampleGetResponse>(
    new SampleGet(postResult.Value!.Result), 
    CancellationToken.None);
getResult.WriteResult<SampleGet, SampleGetResponse>();


var deleteResult = await dispatcher.Dispatch(new SampleDelete(postResult.Value!.Result), CancellationToken.None);
deleteResult.WriteResult<SampleDelete, Empty>();

var dir = Directory.GetCurrentDirectory();
var path = Path.Combine(dir, "sample_upload.txt");
var file = File.OpenRead(path);

var uploadResult = await dispatcher.Dispatch<SampleFileUpload, SampleFileUploadResponse>(
    new SampleFileUpload(file), 
    CancellationToken.None);
uploadResult.WriteResult<SampleFileUpload, SampleFileUploadResponse>();

var downloadResult = await dispatcher.Dispatch<SampleFileDownload, Stream>(
    new SampleFileDownload(uploadResult.Value.CreatedId), 
    CancellationToken.None);
downloadResult.WriteResult<SampleFileDownload, Stream>();
    

