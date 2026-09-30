using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using collageApi.DTOs;

namespace collageApi.Services;

public class PhotoService : IPhotoService
{
    /*private readonly DynamoDBContext _dynamoDbContext;
    private readonly AmazonS3Client _s3Client;
    private readonly AmazonSQSClient _sqsClient;
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(DynamoDBContext dynamoDbContext, AmazonS3Client s3Client, AmazonSQSClient sqsClient, ILogger<PhotoService> logger)
    {
        _dynamoDbContext = dynamoDbContext;
        _s3Client = s3Client;
        _sqsClient = sqsClient;
        _logger = logger;
    }*/
    private readonly IAmazonS3 _amazonS3;
    private readonly IAmazonSQS _amazonSqs;
    private readonly IAmazonDynamoDB _amazonDynamoDb;

    public PhotoService(IAmazonS3 amazonS3, IAmazonSQS amazonSqs, IAmazonDynamoDB amazonDynamoDb)
    {
        _amazonS3 = amazonS3;
        _amazonSqs = amazonSqs;
        _amazonDynamoDb = amazonDynamoDb;
    }

    public async Task<IEnumerable<PhotoDto>> GetAllPhotoInformationFromBucketAsync(string tableName)
    {
        var response = await _amazonDynamoDb.ScanAsync(new ScanRequest
        {
            TableName = tableName
        });

        var photos = response.Items.Select(item => new PhotoDto(
            Id: item["PhotoId"].S,
            Key: item["Key"].S,
            Bucket: item["Bucket"].S
        ));
    
        return photos;
    }
    
    public async Task<Stream> GetPhotoByIdAsync(string photoId, string tableName)
    {
        var response = await _amazonDynamoDb.GetItemAsync(new GetItemRequest
        {
            TableName = tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["PhotoId"] = new(photoId)
            }
        });

        if (response.Item.Count == 0)
        {
            return Stream.Null;
        }

        var request = new GetObjectRequest
            { BucketName = response.Item["Bucket"].S, Key = response.Item["PhotoId"].S };

        var getObjectResponse = await _amazonS3.GetObjectAsync(request);

        var responseStream = getObjectResponse.ResponseStream;

        return responseStream;
    }
    /*
    public async Task<PhotoDto> CreatePhotoAsync(CreatePhotoDto command)
    {
        var photo = Photo.Create(command.S3Location,command.PhotoData);
        
        await _collageDbContext.Photos.AddAsync(photo);
        await _collageDbContext.SaveChangesAsync();
        
        return new PhotoDto(
            photo.Id,
            photo.S3Location,
            photo.PhotoData
            );
    }

    public async Task<PhotoDto?> GetPhotoAsync(Guid id)
    {
        var photo = await _collageDbContext.Photos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (photo is null)
            return null;
        
        return new PhotoDto(
            photo.Id,
            photo.S3Location,
            photo.PhotoData
            );
    }

    public async Task DeletePhotoAsync(Guid id)
    {
        var photoToDelete = await _collageDbContext.Photos.FindAsync(id);

        if (photoToDelete is not null)
        {
            _collageDbContext.Photos.Remove(photoToDelete);
            await _collageDbContext.SaveChangesAsync();
        }
    }*/
}