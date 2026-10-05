using Amazon;
using Amazon.S3;
using collageApi.Configuration;
using Microsoft.Extensions.Options;

namespace collageApi.Services.AWS.Factory;

public class S3ClientFactory : IS3ClientFactory
{
    public IAmazonS3 CreateClient(IOptions<AWSSettings> awsSettings)
    {
        var s3Config = new AmazonS3Config();

        if (!string.IsNullOrWhiteSpace(awsSettings.Value.ServiceUrl))
        {
            s3Config.ServiceURL = awsSettings.Value.ServiceUrl;
            s3Config.ForcePathStyle = true;
            s3Config.UseHttp = true;
        }
        else
        {
            s3Config.RegionEndpoint = RegionEndpoint.GetBySystemName(awsSettings.Value.Region);
        }

        return new AmazonS3Client(s3Config);
    }
   
}