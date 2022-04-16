namespace Maets.Services.Files;

public interface IFileReadService
{
    string GetPublicUrl(string key);

    Task<byte[]> ReadAsync(string key);
}
