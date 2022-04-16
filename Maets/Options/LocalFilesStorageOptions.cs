namespace Maets.Options;

public class LocalFilesStorageOptions
{
    public const string ConfigurationKey = "LocalFilesStorageOptions";

    public const string ResourcesPathKey = $"{ConfigurationKey}:{nameof(ResourcesPath)}";

    public string? ResourcesPath { get; init; }
}
