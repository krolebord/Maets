using Maets.Domain.Entities;

namespace Maets.Services.Files;

public interface IFileWriteService
{
    Task<MediaFile> UploadFileAsync(string key, Stream content);

    Task<MediaFile> UpdateFileAsync(MediaFile oldFileInfo, Stream content);

    Task DeleteFileAsync(MediaFile fileInfo);
}
