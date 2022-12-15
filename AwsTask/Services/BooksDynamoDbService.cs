using AwsTask.Models;

namespace AwsTask.Services;

public interface IBooksDynamoDbService
{
	Task<BookModel> UpsertAsync(BookModel book);

    Task<BookModel> GetAsync(string isbn);

    Task<string> DeleteAsync(string isbn);
}

public class BooksDynamoDbService : IBooksDynamoDbService
{
    private readonly IBooksDynamoDbContext _dbContext;
    private readonly IBookSqsService _sqsService;
    

    public BooksDynamoDbService(IBooksDynamoDbContext dbContext, IBookSqsService sqsService)
    {
        _dbContext = dbContext;
        _sqsService = sqsService;
    }

    public async Task<BookModel> UpsertAsync(BookModel book)
    {
        try
        {
            await _dbContext.SaveAsync(book);
            await _sqsService.SendMessage($"The book with ISBN - {book.Isbn} was added/updated. ");
            return book;
        }
        catch (Exception)
        {
            var errorMessage = $"Failed to add/update a book '{book}'";
            throw new BookDynamoDbServiceException(errorMessage);
        }
    }

    public async Task<BookModel> GetAsync(string isbn)
    {
        var book = await _dbContext.LoadAsync<BookModel>(isbn);
        await _sqsService.SendMessage($"The book with ISBN - {book.Isbn} was requested.");
        return book;
    }

    public async Task<string> DeleteAsync(string isbn)
    {
        await _dbContext.DeleteAsync<BookModel>(isbn);
        await _sqsService.SendMessage($"The book with ISBN - {isbn} was deleted. ");
        return isbn;
    }
}