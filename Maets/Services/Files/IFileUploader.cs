namespace Maets.Services.Files;

public interface IFileUploader
{
    Task UploadAsync(string key, Stream fileContent);

    Task DeleteAsync(string key);
}
