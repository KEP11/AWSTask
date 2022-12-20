using Amazon.DynamoDBv2.DataModel;

namespace AwsTask.Models;

public class BookModel
{
    [DynamoDBHashKey]
    public string Isbn { get; set; }
    
    public string? Title { get; set; }
    
    public string? Description { get; set; }
}
