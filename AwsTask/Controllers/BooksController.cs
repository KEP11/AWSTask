using AwsTask.Models;
using AwsTask.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AwsTask.Controllers;

/// <summary>
/// Controller for Books features.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksDynamoDbService _service;

    public BooksController(IBooksDynamoDbService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets the specified book by the ISBN.
    /// </summary>
    /// <param name="isbn">The ISBN value.</param>
    /// <returns></returns>
    [HttpGet]
    [SwaggerOperation("GetBook")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookModel))]
		public async Task<BookModel> GetBook(string isbn)
    {
        return await _service.GetAsync(isbn);
    }

    /// <summary>
    /// Adds new or updates existing book.
    /// </summary>
    /// <param name="bookModel">The model describing the book.</param>
    /// <returns>Book which was created or updated.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookModel))]
		public async Task<BookModel> AddOrUpdate(BookModel bookModel)
    {
       return await _service.UpsertAsync(bookModel);
    }

    /// <summary>
    /// Deletes the certain book by ISBN value.
    /// </summary>
    /// <param name="isbn">ISBN value.</param>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
		public async Task<string> DeleteBook(string isbn)
    {
        return await _service.DeleteAsync(isbn);
    }
}