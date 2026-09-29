namespace collageApi.Models;

public sealed class Photo
{
    public string PhotoId { get; private set; }
    public string Key { get; private set; }
    public string Bucket { get; private set; }

    private Photo()
    {
        PhotoId = string.Empty;
        Key = string.Empty;
        Bucket = string.Empty;
    }

    private Photo(string photoId, string key, string bucket)
    {
        PhotoId = photoId;
        Key = key;
        Bucket = bucket;
    }

}