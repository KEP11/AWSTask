namespace AwsTask;

public class BookDynamoDbServiceException : ApplicationException
{
    private readonly string _message;

    public override string Message => InnerException == null ? _message : $"{_message} => {InnerException.Message}";
    public BookDynamoDbServiceException(string message)
    {
        _message = $@"{message}. For more information see inner exception.";
    }
}