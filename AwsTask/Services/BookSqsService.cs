using System.Collections.Concurrent;
using Amazon.SQS;
using Amazon.SQS.Model;
using AwsTask.Models;
using Microsoft.Extensions.Options;

namespace AwsTask.Services;

public interface IBookSqsService
{
	Task<SendMessageResponse> SendMessage(string messageBody);
}

public class BookSqsService : IBookSqsService
{
	private readonly IAmazonSQS _sqsClient;
	private readonly string _queueName;

	public BookSqsService(IAmazonSQS sqsClient, IOptions<SqsSettings> sqsSettings)
	{
		_sqsClient = sqsClient;
		_queueName = sqsSettings.Value.QueueName;
	}

	public async Task<SendMessageResponse> SendMessage(string messageBody)
	{
		var queueUrl = await GetQueueUrl();
		var sendMessageRequest = new SendMessageRequest()
		{
			MessageBody = messageBody,
			QueueUrl = queueUrl,
		};
		
		var responseSendMsg = await _sqsClient.SendMessageAsync(sendMessageRequest);
		return responseSendMsg;
	}
	
	private async Task<string> GetQueueUrl()
	{
		if (string.IsNullOrEmpty(_queueName))
		{
			throw new ArgumentException("Queue name should not be blank.");
		}

		try
		{
			var response = await _sqsClient.GetQueueUrlAsync(_queueName);
			return response.QueueUrl;
		}
		catch (QueueDoesNotExistException ex)
		{
			throw new InvalidOperationException($"Could not retrieve the URL for the queue '{_queueName}' as it does not exist or you do not have access to it.", ex);
		}
	}
}