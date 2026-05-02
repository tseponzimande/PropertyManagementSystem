namespace PropertyManagementSystem.UI.Components.Utilities;

public sealed class FormFileWrapper(Stream stream, string fileName, string contentType, long length) : IFormFile
{
    private readonly Stream _stream = stream;

    public string ContentType { get; } = contentType;
    public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{FileName}\"";
    public IHeaderDictionary Headers => new HeaderDictionary();
    public long Length { get; } = length;
    public string Name { get; } = "file";
    public string FileName { get; } = fileName;

    public void CopyTo(Stream target) => _stream.CopyTo(target);
    public Task CopyToAsync(Stream target, CancellationToken c = default) => _stream.CopyToAsync(target, c);
    public Stream OpenReadStream() => _stream;
}
