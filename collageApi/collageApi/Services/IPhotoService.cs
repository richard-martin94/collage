using collageApi.DTOs;

namespace collageApi.Services;

public interface IPhotoService
{
    Task<IEnumerable<PhotoDto>> GetAllPhotoInformationFromBucketAsync();
    Task<Stream> GetPhotoByIdAsync(string photoId);
    Task<IEnumerable<PhotoDto>> PutPhotoAsync();
}