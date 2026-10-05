using collageApi.Services.AWS;
using Amazon.DynamoDBv2.Model;
using Amazon.SQS;
using collageApi.Configuration;
using collageApi.DTOs;
using collageApi.Services.AWS.Factory;
using Microsoft.Extensions.Options;

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
    //private readonly IAmazonS3 _amazonS3;
    //private readonly IAmazonSQS _amazonSqs;
    //private readonly IAmazonDynamoDB _amazonDynamoDb;
    private readonly IS3Service _s3Service;
    private readonly ISqsService _sqsService;
    private readonly IDynamoDbService _dynamoDb;
    private readonly string _bucketName;

    public PhotoService(IS3Service s3Service, ISqsService sqsService, IDynamoDbService dynamoDb, IOptions<AWSSettings> awsSettings)
    {/*
        _amazonS3 = amazonS3;
        _amazonSqs = amazonSqs;
        _amazonDynamoDb = amazonDynamoDb;*/
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
        /*var response = await _amazonDynamoDb.ScanAsync(new ScanRequest
        {
            TableName = tableName
        });

        var photos = response.Items.Select(item => new PhotoDto(
            Id: item["PhotoId"].S,
            Key: item["Key"].S,
            Bucket: item["Bucket"].S
        ));*/
    
        return photos;
    }
    
    public async Task<Stream> GetPhotoByIdAsync(string photoId)
    {
        var item = _dynamoDb.GetItemAsync(photoId).Result;
        /*var response = await _amazonDynamoDb.GetItemAsync(new GetItemRequest
        {
            TableName = tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["PhotoId"] = new(photoId)
            }
        });
        */
        
        if (item.Count == 0)
        {
            return Stream.Null;
        }

        /*
        var request = new GetObjectRequest
            { BucketName = item["Bucket"].S, Key = item["PhotoId"].S };
        //  { BucketName = response.Item["Bucket"].S, Key = response.Item["PhotoId"].S };
        
        var getObjectResponse = await _amazonS3.GetObjectAsync(request);

        var responseStream = getObjectResponse.ResponseStream;
        */
        
        var responseStream = _s3Service.GetItemAsync(photoId).Result;

        return responseStream;
    }

    public async Task<IEnumerable<PhotoDto>> PutPhotoAsync()
    {
        //var bucketName = resourcesSection["BucketName"] ?? "photo-bucket";
        //var tableName = resourcesSection["TableName"] ?? "Photos";
        //var queueName = resourcesSection["QueueName"] ?? "photo-events";

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
            )
        ];
        
        foreach (var photo in photos)
        {
            // 1. Save to DynamoDB
            /*var putRequest = new PutItemRequest
            {
                TableName = tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    ["PhotoId"] = new(photo.Id),
                    ["Key"] = new(photo.Key),
                    ["Bucket"] = new(photo.Bucket),
                    ["CreatedAt"] = new(DateTimeOffset.UtcNow.ToString())
                }
            };
            await _amazonDynamoDb.PutItemAsync(putRequest); */
            var item = new Dictionary<string, AttributeValue>
            {
                ["PhotoId"] = new(photo.Id),
                ["Key"] = new(photo.Key),
                ["Bucket"] = new(photo.Bucket),
                ["CreatedAt"] = new(DateTimeOffset.UtcNow.ToString())
            };
            await _dynamoDb.AddItemAsync(item);
            
            // 2. Upload photo to S3
            /*await using Stream photoSource = File.OpenRead($"/home/richard/Projects/collage/collageApi/collageApi/Photos/{photo.Key}");
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = $"{photo.Id}",
                InputStream = photoSource,
                ContentType = "image/jpeg"
            };
            await _amazonS3.PutObjectAsync(putObjectRequest);*/

            var path = $"/home/richard/Projects/collage/collageApi/collageApi/Photos/{photo.Key}";
            await _s3Service.PutItemAsync(photo.Id, path);
            
            // 3. Send message to SQS
            /*var queueUrlResponse = await _amazonSqs.GetQueueUrlAsync(queueName);
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,
                MessageBody = JsonSerializer.Serialize(photo)
            };
            await _amazonSqs.SendMessageAsync(sendMessageRequest);*/
            await _sqsService.SendMessage(photo);
        }
        return photos;
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