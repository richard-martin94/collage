using collageApi.DTOs;

namespace collageApi.Services.AWS.Factory;

public interface ISqsService
{
    Task SendMessage(PhotoDto photo);
}