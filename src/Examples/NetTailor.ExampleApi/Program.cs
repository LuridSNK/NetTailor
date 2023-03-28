using Microsoft.AspNetCore.StaticFiles;
using NetTailor.Example.Contracts;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var items = new Dictionary<string, IEnumerable<string>>();

app.MapGet("/sample/{id}", (string id) => 
    items.TryGetValue(id, out var itemsById) ? 
        Results.Ok(new SampleGetResponse(itemsById.ToArray())) : 
        Results.NotFound());

app.MapPost("/sample", (HttpContext ctx, SamplePost request) =>
{
    var res = CheckHeaders(ctx);
    if (res is not null) return res;
    var id = Guid.NewGuid().ToString();
    items.Add(id, request.Items);
    return Results.Created($"/sample/{id}", new SamplePostResponse(id));
});

app.MapDelete("/sample/{id}", (HttpContext ctx, string id) =>
{
    var res = CheckHeaders(ctx);
    if (res is not null) return res;
    return !items.Remove(id) ? 
        Results.NotFound() : 
        Results.NoContent();
});

app.MapPost("/files", async (IFormFile file) =>
{
    var dir = Path.Combine(Directory.GetCurrentDirectory(), "temp");
    var id = $"{Guid.NewGuid()}";
    var ext = Path.GetExtension(file.FileName);
    var path = Path.Combine(dir, $"{id}{ext}");
    if (!Directory.Exists(Path.Combine(dir)))
    {
        Directory.CreateDirectory(dir);
    }
    await using var fs = File.Create(path);
    await file.CopyToAsync(fs);
    return Results.Created($"/files/{id}", new SampleFileUploadResponse(id));
});

FileExtensionContentTypeProvider provider = new();
app.MapGet("/files/{id}", async (string id) =>
{
    var dir = Path.Combine(Directory.GetCurrentDirectory(), "temp");
    if (!Directory.Exists(Path.Combine(dir)))
    {
        return Results.NotFound();
    }

    var dirInfo = new DirectoryInfo(dir);
    var files = dirInfo.GetFiles($"{id}.*");
    var file = files.FirstOrDefault();
    if (file is null)
    {
        return Results.NotFound();
    }
    
    var fs = File.OpenRead(file.FullName);
    var type = provider.TryGetContentType(file.FullName, out var contentType) ?
        contentType : 
        "application/octet-stream";
    return Results.File(fs, type);
});

app.Run();


IResult? CheckHeaders(HttpContext ctx)
{
    var authHeader = ctx.Request.Headers.Authorization;
    if (authHeader.FirstOrDefault() != "Bearer 12345")
    {
        return Results.Unauthorized();
    }

    return null;
}