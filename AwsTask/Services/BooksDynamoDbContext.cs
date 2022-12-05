using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace AwsTask.Services;

public interface IBooksDynamoDbContext : IDynamoDBContext
{
}

public class BooksDynamoDbContext : DynamoDBContext, IBooksDynamoDbContext
{
    protected IDynamoDBContext DbContext { get; }
    
    public BooksDynamoDbContext(IAmazonDynamoDB client) : base(client)
    {
        var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
        DbContext = new DynamoDBContext(client, config);
    }
}