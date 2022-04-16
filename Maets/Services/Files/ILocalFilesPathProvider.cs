namespace Maets.Services.Files;

public interface ILocalFilesPathProvider
{
    string ResourcesPath { get; }

    string BuildFullPath(string filePath);
}
