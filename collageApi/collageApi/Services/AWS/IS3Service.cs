namespace collageApi.Services.AWS;

public interface IS3Service
{
    Task <Stream> GetItemAsync(string photoId);
    Task<string> GetPresignedUrlAsync(string key, string verb);
    Task PutItemAsync(string photoId, string path);
}