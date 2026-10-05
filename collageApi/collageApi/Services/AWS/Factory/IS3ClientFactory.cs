using Amazon.S3;
using collageApi.Configuration;
using Microsoft.Extensions.Options;

namespace collageApi.Services.AWS.Factory;

public interface IS3ClientFactory
{
    IAmazonS3 CreateClient(IOptions<AWSSettings> awsSettings);
}