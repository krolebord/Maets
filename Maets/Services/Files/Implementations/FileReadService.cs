using Maets.Attributes;
using Maets.Services.Shared;

namespace Maets.Services.Files.Implementations;

[Dependency(Lifetime = ServiceLifetime.Scoped, Exposes = typeof(IFileReadService))]
public class FileReadService : IFileReadService
{
    private readonly ILocalFilesPathProvider _pathProvider;

    private readonly Lazy<string> _urlPrefix;

    public FileReadService(
        ILocalFilesPathProvider pathProvider,
        IAppUrlProvider urlProvider
    )
    {
        _pathProvider = pathProvider;

        _urlPrefix = new Lazy<string>(urlProvider.GetAppUrl);
    }

    public Task<byte[]> ReadAsync(string key)
    {
        var fullPath = _pathProvider.BuildFullPath(key);

        return File.ReadAllBytesAsync(fullPath);
    }

    public string GetPublicUrl(string key)
    {
        var fullPath = $"{_urlPrefix.Value}/{_pathProvider.BuildFullPath(key)}";

        return fullPath;
    }
}
