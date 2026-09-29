using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.SQS;
using collageApi.Models;
using collageApi.DTOs;
using Microsoft.AspNetCore.Identity;

namespace collageApi.Services;

public class PhotoService : IPhotoService
{
    private readonly DynamoDBContext _dynamoDbContext;
    private readonly AmazonS3Client _s3Client;
    private readonly AmazonSQSClient _sqsClient;
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(DynamoDBContext dynamoDbContext, AmazonS3Client s3Client, AmazonSQSClient sqsClient, ILogger<PhotoService> logger)
    {
        _dynamoDbContext = dynamoDbContext;
        _s3Client = s3Client;
        _sqsClient = sqsClient;
        _logger = logger;
    }

    public async Task<PhotoDto?> GetPhotoByIdAsync(string photoId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PhotoDto>> GetAllPhotoInformationFromBucketAsync()
    {
        throw new NotImplementedException();
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