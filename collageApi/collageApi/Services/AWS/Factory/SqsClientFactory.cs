using Amazon;
using Amazon.DynamoDBv2;
using Amazon.SQS;
using collageApi.Configuration;
using Microsoft.Extensions.Options;

namespace collageApi.Services.AWS.Factory;

public class SqsClientFactory : ISqsClientFactory
{
    public IAmazonSQS CreateClient(IOptions<AWSSettings> awsSettings)
    {
        var sqsConfig = new AmazonSQSConfig();

        if (!string.IsNullOrWhiteSpace(awsSettings.Value.ServiceUrl))
        {
            sqsConfig.ServiceURL = awsSettings.Value.ServiceUrl;
            sqsConfig.UseHttp = true;
        }
        else
        {
            sqsConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(awsSettings.Value.Region);
        }

        return new AmazonSQSClient(sqsConfig);
    }
}