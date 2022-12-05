using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Model;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AWSLambda;

public class Function
{
    /*private readonly IAmazonS3 _s3Client;*/
    
    public Function()
    {
    }

    public async Task FunctionHandler(SQSEvent @event, ILambdaContext context)
    {
        foreach(var message in @event.Records)
        {
            await ProcessMessageAsync(message, context);
        }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processed message {message.Body}");
        await WriteToS3Async(message.Body, context);
        await Task.CompletedTask;
    }
    
    private async Task<bool> WriteToS3Async( string content, ILambdaContext context)
    {
        const string bucketName = "kep-aws-task-bucket";
        var key = $"book_{DateTime.Now.Ticks}.json";
        try
        {
            using var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                ContentBody = content
            };
            var response = await client.PutObjectAsync(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            context.Logger.Log($"{response.HttpStatusCode} encountered putting: {bucketName}:{key}");
            return false;
        }
        catch (Exception ex)
        {
            context.Logger.LogLine("Exception in PutS3Object:" + ex.Message);
            return false;
        }
    }
}