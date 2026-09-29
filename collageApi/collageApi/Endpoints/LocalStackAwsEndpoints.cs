using Amazon.SQS;
using Amazon.SQS.Model;

namespace collageApi.Endpoints;

public static class LocalStackAwsEndpoints
{
    public static void MapLocalStackAwsEndpoints(this IEndpointRouteBuilder routes, IConfigurationSection awsConfigSection)
    {
        var collageApi = routes.MapGroup("/api/v1");

        collageApi.MapGet("/status", () => TypedResults.Ok(new
        {
            Status = "Running",
            Mode = awsConfigSection.GetValue<bool>("UseLocalStack") ? "Localstack" : "AWS",
            Timestamp = DateTime.UtcNow
        }));

        collageApi.MapGet("/messages", async (IAmazonSQS sqs) =>
        {
            var queueUrlResponse = await sqs.GetQueueUrlAsync(awsConfigSection["QueueName"] ?? "photo-events");
            var receiveResponse = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 1
            });

            return Results.Ok(receiveResponse.Messages.Select(m => new
            {
                m.MessageId,
                m.Body,
                m.ReceiptHandle
            }));
        });
    }
}