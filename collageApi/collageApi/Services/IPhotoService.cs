using collageApi.DTOs;

namespace collageApi.Services;

public interface IPhotoService
{
    Task<IEnumerable<PhotoDto>> GetAllPhotoInformationFromBucketAsync(string tableName);
    Task<Stream> GetPhotoByIdAsync(string photoId, string tableName);
    Task<IEnumerable<PhotoDto>> PutPhotoAsync(IConfigurationSection resourcesSection);
}