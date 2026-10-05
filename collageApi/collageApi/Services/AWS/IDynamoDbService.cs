using Amazon.DynamoDBv2.Model;
using collageApi.DTOs;

namespace collageApi.Services.AWS;

public interface IDynamoDbService
{
    Task AddItemAsync(Dictionary<string, AttributeValue> item);
    Task <Dictionary<string, AttributeValue>> GetItemAsync(string photoId);
    Task <IEnumerable<Dictionary<string, AttributeValue>>> AsyncScan();
}