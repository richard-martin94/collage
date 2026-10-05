using Amazon;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.SQS;
using collageApi.Configuration;
using collageApi.Endpoints;
using collageApi.Services;
using collageApi.Services.AWS;
using collageApi.Services.AWS.Factory;
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
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

Console.WriteLine("Environment: " + environment);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

//aws config
var awsSection = builder.Configuration.GetSection("AWS");
/*var region = awsSection["Region"] ?? "us-east-1";
var useLocalStack = awsSection.GetValue<bool>("UseLocalStack");
var serviceUrl = awsSection["ServiceUrl"];*/

//define resources
var resourcesSection = builder.Configuration.GetSection("Resources");

//bind configuration
builder.Services.Configure<AWSSettings>(config.GetSection("AWS"));

//SDK Services
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAWSService<IAmazonSQS>();

//S3 client registration
/*builder.Services.AddSingleton<IAmazonS3>(_ =>
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
});*/

//dynamoDB client registration
/*
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
*/

//sqs client registration
/*
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
*/

//application services
builder.Services.AddScoped<IS3Service, S3Service>();
builder.Services.AddScoped<IS3ClientFactory, S3ClientFactory>();
builder.Services.AddScoped<ISqsService, SqsService>();
builder.Services.AddScoped<ISqsClientFactory, SqsClientFactory>();
builder.Services.AddScoped<IDynamoDbService, DynamoDbService>();
builder.Services.AddScoped<IDynamoDbClientFactory, DynamoDbClientFactory>();

//application layers
builder.Services.AddScoped<IPhotoService, PhotoService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
app.MapPhotoEndpoints();

app.Run();
