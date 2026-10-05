using collageApi.DTOs;

namespace collageApi.Services.AWS;

public interface ISqsService
{
    Task SendMessage(PhotoDto photo);
}