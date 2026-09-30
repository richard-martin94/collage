using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using collageApi.DTOs;
using collageApi.Endpoints;
using collageApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()   
            .AllowAnyMethod()   
            .AllowAnyHeader();
    });
});

//aws config
var awsSection = builder.Configuration.GetSection("AWS");
var region = awsSection["Region"] ?? "us-east-1";
var useLocalStack = awsSection.GetValue<bool>("UseLocalStack");
var serviceUrl = awsSection["ServiceUrl"];

//define resources
var resourcesSection = builder.Configuration.GetSection("Resources");
var bucketName = resourcesSection["BucketName"] ?? "photo-bucket";
var tableName = resourcesSection["TableName"] ?? "Photos";
var queueName = resourcesSection["QueueName"] ?? "photo-events";

//S3 client registration
builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var config = new AmazonS3Config
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(region)
    };
    if (useLocalStack && !string.IsNullOrEmpty(serviceUrl))
    {
        config.ServiceURL = serviceUrl;
        config.ForcePathStyle = true; //required to work with local stack s3
        config.UseHttp = true;
    }

    return new AmazonS3Client(config);
});

//dynamoDB client registration
builder.Services.AddSingleton<IAmazonDynamoDB>(_ =>
{
    var config = new AmazonDynamoDBConfig
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(region)
    };

    if (useLocalStack && !string.IsNullOrEmpty(serviceUrl))
    {
        config.ServiceURL = serviceUrl;
        config.UseHttp = true;
    }

    return new AmazonDynamoDBClient(config);
});

//sqs client registration
builder.Services.AddSingleton<IAmazonSQS>(_ =>
{
    var config = new AmazonSQSConfig
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(region)
    };

    if (useLocalStack && !string.IsNullOrEmpty(serviceUrl))
    {
        config.ServiceURL = serviceUrl;
        config.UseHttp = true;
    }

    return new AmazonSQSClient(config);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddTransient<IPhotoService, PhotoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseCors(); 
}

app.UseHttpsRedirection();

app.MapLocalStackAwsEndpoints(awsSection);
app.MapPhotoEndpoints(resourcesSection);

/* moved to endpoints: MapLocalStackAwsEndpoints
 app.MapGet("/", () => Results.Ok(new
{
    Status = "Running",
    Mode = useLocalStack ? "Localstack" : "AWS",
    Timestamp = DateTime.UtcNow
}));*/

app.MapPost("/photos", async (
    IAmazonDynamoDB dynamoDb,
    IAmazonS3 s3,
    IAmazonSQS sqs) =>
{
    var photo = new PhotoDto(
        Id: Guid.NewGuid().ToString(),
        Key: "testImage.jpg",
        Bucket: bucketName
    );

    // 1. Save to DynamoDB
    var putRequest = new PutItemRequest
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
    await dynamoDb.PutItemAsync(putRequest);

    // 2. Upload photo to S3
    var photoId = photo.Id;
    await using Stream photoSource = File.OpenRead("/home/richard/Projects/collage/collageApi/collageApi/Photos/testImage.jpg");
    var putObjectRequest = new PutObjectRequest
    {
        BucketName = bucketName,
        Key = $"{photoId}",
        InputStream = photoSource,
        ContentType = "image/jpeg"
    };
    await s3.PutObjectAsync(putObjectRequest);

    // 3. Send message to SQS
    var queueUrlResponse = await sqs.GetQueueUrlAsync(queueName);
    var sendMessageRequest = new SendMessageRequest
    {
        QueueUrl = queueUrlResponse.QueueUrl,
        MessageBody = JsonSerializer.Serialize(photo)
    };
    await sqs.SendMessageAsync(sendMessageRequest);

    return Results.Created($"/photos/{photo.Id}", photo);
});

/*
 these get requests were chopped up and moved to IPhotoService, PhotoService and PhotoEndpoints
//list information for all photos in bucket
app.MapGet("/photos", async (IAmazonDynamoDB dynamoDb) =>
{
    var response = await dynamoDb.ScanAsync(new ScanRequest
    {
        TableName = tableName
    });

    var photos = response.Items.Select(item => new PhotoDto(
        Id: item["PhotoId"].S,
        Key: item["Key"].S,
        Bucket: item["Bucket"].S
    ));
    
    return Results.Ok(photos);
});*/

//returns an image file given a photoid
/*app.MapGet("/photos/{photoId}", async (string photoId, IAmazonDynamoDB dynamoDb, IAmazonS3 s3) =>
{
    var response = await dynamoDb.GetItemAsync(new GetItemRequest
    {
        TableName = tableName,
        Key = new Dictionary<string, AttributeValue>
        {
            ["PhotoId"] = new(photoId)
        }
    });

    if (response.Item.Count == 0)
        return Results.NotFound(new { Message = "photo not found" });
    
    //return Results.Ok(response);

    var request = new GetObjectRequest
        { BucketName = response.Item["Bucket"].S, Key = response.Item["PhotoId"].S };

    using var getObjectResponse = await s3.GetObjectAsync(request);

    //await getObjectResponse.WriteResponseStreamToFileAsync("/home/richard/Downloads/testRetrieve2.jpeg", true, CancellationToken.None);
    
    return Results.File(getObjectResponse.ResponseStream, "image/jpeg");
});*/

/* moved to endpoints: MapLocalStackAwsEndpoints
        check sqs messages for debugging
app.MapGet("/messages", async (IAmazonSQS sqs) =>
{
    var queueUrlResponse = await sqs.GetQueueUrlAsync(queueName);
    var receiveResponse = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest
    {
        QueueUrl = queueUrlResponse.QueueUrl,
        MaxNumberOfMessages = 10,
        WaitTimeSeconds = 1
    });

    return Results.Ok(receiveResponse.Messages.Select(m => new
    {
        m.MessageId,
        m.Body,
        m.ReceiptHandle
    }));
});*/

app.Run();
