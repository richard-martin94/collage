namespace collageApi.Services.AWS;

public interface IS3Service
{
    Task <Stream> GetItemAsync(string photoId);
    Task<string> GetPresignedUrlAsync(string key);
    Task PutItemAsync(string photoId, string path);
}