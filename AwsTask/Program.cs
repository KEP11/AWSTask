using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using AwsTask.Extensions;
using AwsTask.Models;
using Microsoft.Extensions.Configuration;

SetAwsCredentials();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;
// Add services to the container.

BuildConfiguration(environment);

var services = builder.Services;
services.Configure<SqsSettings>(configuration.GetSection(nameof(SqsSettings)));

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddAWSService<IAmazonSQS>();
services.AddAWSService<IAmazonS3>();

services.AddDynamoDbBooksService(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void SetAwsCredentials()
{
	Console.WriteLine("Setting AWS credentials.");
	var chain = new CredentialProfileStoreChain();

	if (!chain.TryGetAWSCredentials("freeacc", out var awsCredentials))
	{
		throw new Exception("Could not get AWS credentials.");
	}

	var credentials = awsCredentials.GetCredentials();

	Environment.SetEnvironmentVariable(EnvironmentVariablesAWSCredentials.ENVIRONMENT_VARIABLE_ACCESSKEY, credentials.AccessKey);
	Environment.SetEnvironmentVariable(EnvironmentVariablesAWSCredentials.ENVIRONMENT_VARIABLE_SECRETKEY, credentials.SecretKey);
	Environment.SetEnvironmentVariable(EnvironmentVariableAWSRegion.ENVIRONMENT_VARIABLE_REGION, RegionEndpoint.USEast1.SystemName);

	Console.WriteLine("Setting AWS credentials finished.");
}

static void BuildConfiguration(IHostEnvironment env)
{
	var configurationBuilder = new ConfigurationBuilder()
		.SetBasePath(Directory.GetCurrentDirectory())
		.AddJsonFile("./appsettings.json", optional: false, reloadOnChange: true)
		.AddJsonFile($"./Configuration/appsettings.{env.EnvironmentName}.json", optional: true)
	.AddEnvironmentVariables();




}
