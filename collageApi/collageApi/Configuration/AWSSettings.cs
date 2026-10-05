namespace collageApi.Configuration;

public class AWSSettings
{
    public string Region { get; set; } = string.Empty;
    public string UseLocalStack { get; set; } = string.Empty;
    public string ServiceUrl { get; set; } = string.Empty;
    public string S3BucketName { get; set; } = string.Empty;
    public string SqsQueueName { get; set; } = string.Empty;
    public string DynamoDbTableName { get; set; } = string.Empty;
    public string ApiKeySecretId { get; set; } = string.Empty;
}