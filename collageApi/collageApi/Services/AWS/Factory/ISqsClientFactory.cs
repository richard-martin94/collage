using Amazon.SQS;
using collageApi.Configuration;
using Microsoft.Extensions.Options;

namespace collageApi.Services.AWS.Factory;

public interface ISqsClientFactory
{
    IAmazonSQS CreateClient(IOptions<AWSSettings> awsSettings);
}