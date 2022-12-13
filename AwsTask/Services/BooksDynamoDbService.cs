using AwsTask.Models;

namespace AwsTask.Services;

public interface IBooksDynamoDbService
{
    Task UpsertAsync(BookModel book);

    Task<BookModel> GetAsync(string isbn);
    
    Task DeleteAsync(string isbn);
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

    public async Task UpsertAsync(BookModel book)
    {
        try
        {
            await _dbContext.SaveAsync(book);
            await _sqsService.SendMessage($"The book with ISBN - {book.Isbn} was added/updated. ");
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

    public async Task DeleteAsync(string isbn)
    {
        await _dbContext.DeleteAsync<BookModel>(isbn);
        await _sqsService.SendMessage($"The book with ISBN - {isbn} was deleted. ");
    }
}