using Amazon.DynamoDBv2;
using collageApi.Configuration;
using Microsoft.Extensions.Options;

namespace collageApi.Services.AWS.Factory;

public interface IDynamoDbClientFactory
{
    IAmazonDynamoDB CreateClient(IOptions<AWSSettings> awsSettings);
}