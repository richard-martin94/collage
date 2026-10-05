using collageApi.Configuration;
using collageApi.DTOs;
using collageApi.Services;
using Microsoft.Extensions.Options;

namespace collageApi.Endpoints;

public static class PhotoEndpoints
{
    public static void MapPhotoEndpoints(this IEndpointRouteBuilder routes)
    {
        
        var collageApi = routes.MapGroup("api/v1").WithTags("Photos");

        collageApi.MapPost("/photos", async (IPhotoService service) =>
        {
            IEnumerable<PhotoDto> photos = await service.PutPhotoAsync();
            //var tableName = resourcesSection["TableName"] ?? "Photos";
            
            return photos is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Something went wrong with put photos request." })
                : TypedResults.Created("", photos);
        });

        collageApi.MapGet("/photos", async (IPhotoService service) =>
        {
            //var tableName = resourcesSection["TableName"] ?? "Photos";
            IEnumerable<PhotoDto> photos = await service.GetAllPhotoInformationFromBucketAsync();
            
            return photos is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Photos not found." })
                : TypedResults.Ok(photos);
        });

        collageApi.MapGet("/photos/{photoId}", async (IPhotoService service, string photoId) =>
        {  
            //var tableName = resourcesSection["TableName"] ?? "Photos";

            var photoResponse = await service.GetPhotoByIdAsync(photoId);

            return photoResponse is null 
                ? (IResult)TypedResults.NotFound(new { Message = $"Photo with id: {photoId} not found." })
                : TypedResults.File(photoResponse, "image/jpeg");
        });

        collageApi.MapDelete("/{photoId}", async (IPhotoService service, Guid photoId) =>
        {
            /*await service.DeletePhotoAsync(photoId);
            return TypedResults.NoContent();*/
            
            throw new NotImplementedException();
        });
    }
}