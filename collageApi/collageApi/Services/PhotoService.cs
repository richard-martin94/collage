using collageApi.Models;
using collageApi.DTOs;
using Microsoft.AspNetCore.Identity;

namespace collageApi.Services;

public class PhotoService : IPhotoService
{
    /*private readonly CollageDbContext _collageDbContext;
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(CollageDbContext collageDbContext, ILogger<PhotoService> logger)
    {
        _collageDbContext = collageDbContext;
        _logger = logger;
    }

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