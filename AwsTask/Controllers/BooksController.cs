using AwsTask.Models;
using AwsTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace AwsTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksDynamoDbService _service;

    public BooksController(IBooksDynamoDbService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<BookModel> GetBooks(string isbn)
    {
        return await _service.GetAsync(isbn);
    }

    [HttpPost]
    public async Task AddOrUpdate(BookModel bookModel)
    {
        await _service.UpsertAsync(bookModel);
    }

    [HttpDelete]
    public async Task DeleteBook(string isbn)
    {
        await _service.DeleteAsync(isbn);
    }
}