using Amazon;
using Amazon.DynamoDBv2;
using AwsTask.Models;
using AwsTask.Services;

namespace AwsTask.Extensions;

public static class BooksDynamoDbServiceExtension
{
    private const string DynamoDbConfiguration = nameof(DynamoDbConfiguration);

    public static IServiceCollection AddDynamoDbBooksService(this IServiceCollection services, IConfiguration configuration)
    {
	    var client = configuration.GetAWSOptions().CreateServiceClient<IAmazonDynamoDB>();
	    services.AddScoped<IBooksDynamoDbContext>(provider => new BooksDynamoDbContext(client));

	    /*services.AddTransient<IBooksDynamoDbContext, BooksDynamoDbContext>();*/
	    services.AddTransient<IBookSqsService, BookSqsService>();
	    services.AddTransient<IBooksDynamoDbService, BooksDynamoDbService>();

	    var bookTable = configuration.GetSection("DynamoDbConfiguration:BookInfo").Value;

	    AWSConfigsDynamoDB.Context.TypeMappings[typeof(BookModel)] = new Amazon.Util.TypeMapping(typeof(BookModel), bookTable);

	    return services;
    }
}