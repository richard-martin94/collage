using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Amazon.SQS.Model;
using collageApi.DTOs;
using collageApi.Services;

namespace collageApi.Endpoints;

public static class PhotoEndpoints
{
    public static void MapPhotoEndpoints(this IEndpointRouteBuilder routes)
    {
        var collageApi = routes.MapGroup("api/v1").WithTags("Photos");

        /*collageApi.MapPost("/photos", async (IPhotoService service) =>
        {
            IEnumerable<PhotoDto> photos = await service.PutPhotoAsync();
            
            return photos is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Something went wrong with put photos request." })
                : TypedResults.Created("", photos);
        });*/

        collageApi.MapGet("/photos", async (IPhotoService service) =>
        {
            IEnumerable<PhotoDto> photos = await service.GetAllPhotoInformationFromBucketAsync();
            
            return photos is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Photos not found." })
                : TypedResults.Ok(photos);
        });

        /*collageApi.MapGet("/photos/{photoId}", async (IPhotoService service, string photoId) =>
        {  
            var photoResponse = await service.GetPhotoByIdAsync(photoId);

            return photoResponse is null 
                ? (IResult)TypedResults.NotFound(new { Message = $"Photo with id: {photoId} not found." })
                : TypedResults.File(photoResponse, "image/jpeg");
        });*/
        
        collageApi.MapGet("/photos/presigned/{verb}/{photoId}", async (IPhotoService service, HttpRequest request, IConfiguration config,  string photoId, string verb) =>
        {
            if (!verb.Equals("GET") || !verb.Equals("PUT"))
            {
                return Results.BadRequest(new { Message = "Missing or incorrect verbs." });
            }

            if (verb.Equals("PUT"))
            {
                var expected = config["AdminKey"];
                var supplied = request.Headers["X-Admin-Key"].ToString();

                var ok = !string.IsNullOrEmpty(expected)
                         && supplied.Length > 0
                         && CryptographicOperations.FixedTimeEquals(
                             Encoding.UTF8.GetBytes(supplied), 
                             Encoding.UTF8.GetBytes(expected));

                if (!ok)
                    return Results.Unauthorized();
            }
            
            var urlResponse = await service.GetPresignedUrl(photoId, verb);
            
            return urlResponse is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Photos not found or failed to upload, see error for more details." })
                : TypedResults.Ok(urlResponse);
        });

        collageApi.MapDelete("/{photoId}", async (IPhotoService service, Guid photoId) =>
        {
            throw new NotImplementedException();
        });
    }
}