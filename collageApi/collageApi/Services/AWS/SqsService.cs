using System.Text.Json;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.Model;
using collageApi.Configuration;
using collageApi.DTOs;
using collageApi.Services.AWS.Factory;
using Microsoft.Extensions.Options;

namespace collageApi.Services.AWS;

public class SqsService : ISqsService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueName;

    public SqsService(ISqsClientFactory clientFactory, IOptions<AWSSettings> awsSettings)
    {
        _sqsClient = clientFactory.CreateClient(awsSettings);
        _queueName = awsSettings.Value.SqsQueueName;
    }
    
    public async Task SendMessage(PhotoDto photo)
    {
        try
        {
            var queueUrlResponse = await _sqsClient.GetQueueUrlAsync(_queueName);
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,
                MessageBody = JsonSerializer.Serialize(photo)
            };
            await _sqsClient.SendMessageAsync(sendMessageRequest);
        }
        catch (AmazonSQSException s3Exception)
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