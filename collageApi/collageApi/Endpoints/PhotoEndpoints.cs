using collageApi.DTOs;
using collageApi.Services;

namespace collageApi.Endpoints;

public static class PhotoEndpoints
{
    public static void MapPhotoEndpoints(this IEndpointRouteBuilder routes, IConfigurationSection resourcesSection)
    {
        var collageApi = routes.MapGroup("api/v1").WithTags("Photos");

        collageApi.MapPost("/photos", async (IPhotoService service) =>
        {
            IEnumerable<PhotoDto> photos = await service.PutPhotoAsync(resourcesSection);
            var tableName = resourcesSection["TableName"] ?? "Photos";
            
            return photos is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Something went wrong putting photos into {tableName}." })
                : TypedResults.Created("", photos);

        });

        collageApi.MapGet("/photos", async (IPhotoService service) =>
        {
            var tableName = resourcesSection["TableName"] ?? "Photos";
            IEnumerable<PhotoDto> photos = await service.GetAllPhotoInformationFromBucketAsync(tableName);
            
            return photos is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Photos in {tableName} not found." })
                : TypedResults.Ok(photos);
        });

        collageApi.MapGet("/photos/{photoId}", async (IPhotoService service, string photoId) =>
        {  
            var tableName = resourcesSection["TableName"] ?? "Photos";

            var photoResponse = await service.GetPhotoByIdAsync(photoId, tableName);

            return photoResponse is null 
                ? (IResult)TypedResults.NotFound(new { Message = $"Photo with id: {photoId} not found in {tableName}." })
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