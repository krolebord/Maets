using Maets.Attributes;
using Maets.Data;
using Maets.Domain.Entities;
using Maets.Extensions;

namespace Maets.Services.Files.Implementations;

[Dependency(Lifetime = ServiceLifetime.Scoped, Exposes = typeof(IFileWriteService))]
public class FileWriteService : IFileWriteService
{
    private readonly IFileUploader _fileUploader;
    private readonly MaetsDbContext _dbContext;

    public FileWriteService(IFileUploader fileUploader, MaetsDbContext dbContext)
    {
        _fileUploader = fileUploader;
        _dbContext = dbContext;
    }

    public async Task<MediaFile> UploadFileAsync(string key, Stream content)
    {
        await _fileUploader.UploadAsync(key, content);

        var file = new MediaFile
        {
            Id = Guid.NewGuid(),
            Key = key
        };

        _dbContext.MediaFiles.Add(file);
        await _dbContext.SaveChangesAsync();

        return file;
    }

    public async Task<MediaFile> UpdateFileAsync(MediaFile file, Stream content)
    {
        await _fileUploader.DeleteAsync(file.Key);

        await _fileUploader.UploadAsync(file.Key, content);

        return file;
    }

    public async Task DeleteFileAsync(MediaFile file)
    {
        await _fileUploader.DeleteAsync(file.Key);

        _dbContext.MediaFiles.Remove(file);
        await _dbContext.SaveChangesAsync();
    }
}
