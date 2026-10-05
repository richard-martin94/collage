using Amazon;
using Amazon.DynamoDBv2;
using collageApi.Configuration;
using Microsoft.Extensions.Options;

namespace collageApi.Services.AWS.Factory;

public class DynamoDbClientFactory : IDynamoDbClientFactory
{
    public IAmazonDynamoDB CreateClient(IOptions<AWSSettings> awsSettings)
    {
        var dynamoDbConfig = new AmazonDynamoDBConfig();

        if (!string.IsNullOrWhiteSpace(awsSettings.Value.ServiceUrl))
        {
            dynamoDbConfig.ServiceURL = awsSettings.Value.ServiceUrl;
            dynamoDbConfig.UseHttp = true;
        }
        else
        {
            dynamoDbConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(awsSettings.Value.Region);
        }

        return new AmazonDynamoDBClient(dynamoDbConfig);
    }
}