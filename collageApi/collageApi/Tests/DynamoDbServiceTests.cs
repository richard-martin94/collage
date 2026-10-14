using System.Runtime.InteropServices.JavaScript;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using collageApi.Configuration;
using collageApi.Services.AWS;
using collageApi.Services.AWS.Factory;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace collageApi.Tests;

public class DynamoDbServiceTests
{
    private readonly IAmazonDynamoDB _subAmazondynamoDb;
    private readonly DynamoDbService _dynamoDbService;
    
    public DynamoDbServiceTests()
    {
        _subAmazondynamoDb = Substitute.For<IAmazonDynamoDB>();
        IDynamoDbClientFactory _subDynamoClientFactory = Substitute.For<IDynamoDbClientFactory>();
        
        //configure test settings
        var settings = new AWSSettings
            {
                ServiceUrl = "http://localhost:8000",
                DynamoDbTableName  = "TestTable"
            };
        IOptions<AWSSettings> _subAwsSettings = Options.Create(settings);
        
        //setup factory
        _subDynamoClientFactory
            .CreateClient(_subAwsSettings)
            .Returns(_subAmazondynamoDb);

        //create substitute(mock) service
        _dynamoDbService = new DynamoDbService(_subDynamoClientFactory, _subAwsSettings);
    }

    [Fact]
    public async Task AddItem_When_InputIsValid_SuccessfullyAddItem()
    {
        //Arrange
        var substituteItem = new Dictionary<string, AttributeValue>
        {
            ["PhotoId"] = new(string.Empty),
            ["Key"] = new("Test Key"),
            ["Bucket"] = new("Test Bucket"),
            ["CreatedAt"] = new(DateTimeOffset.UtcNow.ToString())
        };

        _subAmazondynamoDb
            .PutItemAsync(Arg.Any<PutItemRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PutItemResponse());

        //act
        await _dynamoDbService.AddItemAsync(substituteItem);
        
        //assert
        await _subAmazondynamoDb
            .Received(1)
            .PutItemAsync(Arg.Is<PutItemRequest>
                (req =>
                    req.Item != null
                    
                    && req.Item["PhotoId"].S == string.Empty
                    && req.Item["Key"].S == "Test Key"
                    && req.Item["Bucket"].S == "Test Bucket"
                    
                    && req.TableName == "TestTable"
                ),Arg.Any<CancellationToken>()
            );
    }
}