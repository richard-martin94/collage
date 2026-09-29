using collageApi.DTOs;

namespace collageApi.Services;

public interface IPhotoService
{
    Task<PhotoDto?> GetPhotoByIdAsync(string photoId);
    Task<IEnumerable<PhotoDto>> GetAllPhotoInformationFromBucketAsync();
    
}