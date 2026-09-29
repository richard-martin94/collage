using collageApi.DTOs;
using collageApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace collageApi.Endpoints;

public static class PhotoEndpoints
{
    public static void MapPhotoEndpoints(this IEndpointRouteBuilder routes)
    {
        var collageApi = routes.MapGroup("api/v1").WithTags("Photos");

        collageApi.MapPost("/", async (IPhotoService service, CreatePhotoDto command) =>
        {
            throw new NotImplementedException();
            /*var photo = await service.CreatePhotoAsync(command);
            return TypedResults.Created($"/api/photos/{photo.Id}", photo);*/

        });

        collageApi.MapGet("/photos", async (IPhotoService service) =>
        {
            throw new NotImplementedException();
        });

        collageApi.MapGet("/photos/{photoId}", async (IPhotoService service, Guid photoId) =>
        {/*
            var photo = await service.GetPhotoAsync(photoId);

            return photo is null
                ? (IResult)TypedResults.NotFound(new { Message = $"photo with id {photoId} not found" })
                : TypedResults.Ok(photo);*/
            
            throw new NotImplementedException();
        });

        collageApi.MapDelete("/{photoId}", async (IPhotoService service, Guid photoId) =>
        {
            /*await service.DeletePhotoAsync(photoId);
            return TypedResults.NoContent();*/
            
            throw new NotImplementedException();
        });
    }
}