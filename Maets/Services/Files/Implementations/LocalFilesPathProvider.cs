using Maets.Attributes;
using Maets.Options;
using Microsoft.Extensions.Options;

namespace Maets.Services.Files.Implementations;

[Dependency(Lifetime = ServiceLifetime.Scoped, Exposes = typeof(ILocalFilesPathProvider))]
public class LocalFilesPathProvider : ILocalFilesPathProvider
{
    public string ResourcesPath { get; private set; }

    public LocalFilesPathProvider(
        IOptions<LocalFilesStorageOptions> storageOptions
    )
    {
        ResourcesPath = storageOptions.Value.ResourcesPath ?? throw new NullReferenceException();

        Directory.CreateDirectory(ResourcesPath);
    }

    public string BuildFullPath(string filePath)
    {
        return $"{ResourcesPath}/{filePath}";
    }
}
