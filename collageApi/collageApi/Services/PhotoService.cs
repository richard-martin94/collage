using collageApi.Services.AWS;
using Amazon.DynamoDBv2.Model;
using Amazon.S3.Model;
using collageApi.Configuration;
using collageApi.DTOs;
using Microsoft.Extensions.Options;

namespace collageApi.Services;

public sealed class PhotoService : IPhotoService
{
    private readonly IS3Service _s3Service;
    private readonly ISqsService _sqsService;
    private readonly IDynamoDbService _dynamoDb;
    private readonly string _bucketName;

    public PhotoService(IS3Service s3Service, ISqsService sqsService, IDynamoDbService dynamoDb, IOptions<AWSSettings> awsSettings)
    {
        _s3Service = s3Service;
        _sqsService = sqsService;
        _dynamoDb = dynamoDb;
        _bucketName = awsSettings.Value.S3BucketName;
    }

    public async Task<IEnumerable<PhotoDto>> GetAllPhotoInformationFromBucketAsync()
    {
        var responseItems = await _dynamoDb.AsyncScan();
        var photos = responseItems.Select(item => new PhotoDto(
            Id: item["PhotoId"].S,
            Key: item["Key"].S,
            Bucket: item["Bucket"].S
        ));
    
        return photos;
    }
    
    /*public async Task<Stream> GetPhotoByIdAsync(string photoId)
    {
        var item = _dynamoDb.GetItemAsync(photoId).Result;
        
        if (item.Count == 0)
        {
            return Stream.Null;
        }
        
        var responseStream = await _s3Service.GetItemAsync(photoId);

        return responseStream;
    }*/

    public async Task<string> GetPresignedUrl(string photoId, string verb)
    {
        var presignedUrl = "";
        
        if (photoId.Contains("."))
        {
            // 1. Save to DynamoDB
            var item = new Dictionary<string, AttributeValue>
            {
                ["PhotoId"] = new(Guid.NewGuid().ToString()),
                ["Key"] = new(photoId),
                ["Bucket"] = new(_bucketName),
                ["CreatedAt"] = new(DateTimeOffset.UtcNow.ToString())
            };
            await _dynamoDb.AddItemAsync(item);
            
            presignedUrl = await _s3Service.GetPresignedUrlAsync(item["PhotoId"].S, verb);
            var photo = new PhotoDto 
            (Id: item["PhotoId"].S, Key: photoId, Bucket: _bucketName);
            
            await _sqsService.SendMessage(photo);
        }
        else
        {
            presignedUrl = await _s3Service.GetPresignedUrlAsync(photoId, verb);
        }
        
        return presignedUrl;
    }

    /*public async Task<IEnumerable<PhotoDto>> PutPhotoAsync()
    {
        List<PhotoDto> photos = 
        [
            new (
                Id: Guid.NewGuid().ToString(),
                Key: "testImage.jpg",
                Bucket: _bucketName
            ),
            new (
            Id: Guid.NewGuid().ToString(),
            Key: "testImage2.jpg",
            Bucket: _bucketName
            ),
            new (
                Id: Guid.NewGuid().ToString(),
                Key: "testImage3.jpg",
                Bucket: _bucketName
            )
        ];
        
        foreach (var photo in photos)
        {
            // 1. Save to DynamoDB
            var item = new Dictionary<string, AttributeValue>
            {
                ["PhotoId"] = new(photo.Id),
                ["Key"] = new(photo.Key),
                ["Bucket"] = new(photo.Bucket),
                ["CreatedAt"] = new(DateTimeOffset.UtcNow.ToString())
            };
            await _dynamoDb.AddItemAsync(item);
            
            // 2. Upload photo to S3
            var path = $"/home/richard/Projects/collage/collageApi/collageApi/Photos/{photo.Key}";
            await _s3Service.PutItemAsync(photo.Id, path);
            
            // 3. Send message to SQS
            await _sqsService.SendMessage(photo);
        }
        return photos;
    }*/
}