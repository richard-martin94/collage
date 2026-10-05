using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using collageApi.Services.AWS.Factory;
using collageApi.Configuration;
using Microsoft.Extensions.Options;

namespace collageApi.Services.AWS;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    
    public S3Service(IS3ClientFactory clientFactory, IOptions<AWSSettings> awsSettings)
    {
        _s3Client = clientFactory.CreateClient(awsSettings);
        _bucketName = awsSettings.Value.S3BucketName;
    }
    
    public async Task <Stream> GetItemAsync(string photoId)
    {
        var request = new GetObjectRequest
            { BucketName = _bucketName, Key = photoId };

        var getObjectResponse = await _s3Client.GetObjectAsync(request);

        return getObjectResponse.ResponseStream;
    }
    
    public async Task PutItemAsync(string photoId, string path)
    {
        try
        {
            await using Stream photoSource = File.OpenRead(path);
            
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = photoId,
                InputStream = photoSource,
                ContentType = "image/jpeg"
            };
            await _s3Client.PutObjectAsync(putObjectRequest); 
        }
        catch (AmazonS3Exception s3Exception)
        {
            Console.WriteLine($"S3 Error: {s3Exception.Message}");
            throw;
        }
        catch (AmazonServiceException serviceException)
        {
            Console.WriteLine($"AWS Service Error: {serviceException.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }
}