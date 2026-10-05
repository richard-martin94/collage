using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using collageApi.Configuration;
using collageApi.Services.AWS.Factory;
using Microsoft.Extensions.Options;

namespace collageApi.Services.AWS;

public class DynamoDbService : IDynamoDbService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;

    public DynamoDbService(IDynamoDbClientFactory clientFactory, IOptions<AWSSettings> awsSettings)
    {
        _dynamoDb = clientFactory.CreateClient(awsSettings);
        _tableName = awsSettings.Value.DynamoDbTableName;
    }

    public async Task AddItemAsync(Dictionary<string, AttributeValue> item)
    {
        if (item.Count < 1)
        {
            throw new ArgumentException("item has no key value pairs.");
        }
        var putRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = item
        };
        try
        {
            await _dynamoDb.PutItemAsync(putRequest); 
        }
        catch (AmazonDynamoDBException ddbException)
        {
            Console.WriteLine($"DynamoDB Error: {ddbException.Message}");
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

    public async Task <Dictionary<string, AttributeValue>> GetItemAsync(string photoId)
    {
        GetItemResponse response;
        var request = new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["PhotoId"] = new(photoId)
            }
        };
        try
        {
            response = await _dynamoDb.GetItemAsync(request);
        }
        catch (AmazonDynamoDBException ddbException)
        {
            Console.WriteLine($"DynamoDB Error: {ddbException.Message}");
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

        return response.Item;
    }

    public async Task <IEnumerable<Dictionary<string, AttributeValue>>> AsyncScan()
    {
        ScanResponse response;
        try
        {
            response = await _dynamoDb.ScanAsync(new ScanRequest
            {
                TableName = _tableName
            });
        }
        catch (AmazonDynamoDBException ddbException)
        {
            Console.WriteLine($"DynamoDB Error: {ddbException.Message}");
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

        return response.Items;
    }
}