namespace collageApi.Services.AWS;

public interface IS3Service
{
    Task <Stream> GetItemAsync(string photoId);
    Task PutItemAsync(string photoId, string path);
}