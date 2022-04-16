using Maets.Attributes;

namespace Maets.Services.Files.Implementations;

[Dependency(Lifetime = ServiceLifetime.Scoped, Exposes = typeof(IFileUploader))]
public class FileUploader : IFileUploader
{
    private readonly ILocalFilesPathProvider _pathProvider;

    public FileUploader(ILocalFilesPathProvider pathProvider)
    {
        _pathProvider = pathProvider;
    }

    public async Task UploadAsync(string key, Stream fileContent)
    {
        if (fileContent.Length == 0)
        {
            return;
        }
        var fullPath = _pathProvider.BuildFullPath(key);

        var directoryName = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        await using var fileStream = new FileStream(fullPath, FileMode.OpenOrCreate);
        await fileContent.CopyToAsync(fileStream);
    }

    public Task DeleteAsync(string key)
    {
        var fullPath = _pathProvider.BuildFullPath(key);

        if(!File.Exists(fullPath))
        {
            return Task.CompletedTask;
        }

        File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
