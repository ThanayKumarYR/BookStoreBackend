using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using ModelLayer.ResponseModel;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookBusinessLayer _bookBusinessLayer;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookBusinessLayer bookBusinessLayer, ILogger<BookController> logger)
        {
            _bookBusinessLayer = bookBusinessLayer;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ResponseModel<int>>> CreateBook([FromBody] Book book)
        {
            _logger.LogInformation("Creating a new book");
            var response = new ResponseModel<int>();

            try
            {
                var bookId = await _bookBusinessLayer.AddBookAsync(book);
                response.Success = true;
                response.Message = "Book created successfully";
                response.Data = bookId;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                response.Success = false;
                response.Message = "Failed to create book";
                return BadRequest(response);
            }
        }

        [HttpGet("getall")]
        public async Task<ActionResult<ResponseModel<List<Book>>>> GetAllBooks()
        {
            _logger.LogInformation("Getting all books");
            var response = new ResponseModel<List<Book>>();

            try
            {
                var books = await _bookBusinessLayer.GetAllBooksAsync();
                response.Success = true;
                response.Message = "Books retrieved successfully";
                response.Data = books;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books");
                response.Success = false;
                response.Message = "Failed to retrieve books";
                return BadRequest(response);
            }
        }

        [HttpGet("getbyid/{bookId}")]
        public async Task<ActionResult<ResponseModel<Book>>> GetBookById(int bookId)
        {
            _logger.LogInformation($"Getting book with ID {bookId}");
            var response = new ResponseModel<Book>();

            try
            {
                var book = await _bookBusinessLayer.GetBookByIdAsync(bookId);
                if (book == null)
                {
                    response.Success = false;
                    response.Message = "Book not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Message = "Book retrieved successfully";
                response.Data = book;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book");
                response.Success = false;
                response.Message = "Failed to retrieve book";
                return BadRequest(response);
            }
        }

        [HttpPut("update/{bookId}")]
        public async Task<ActionResult<ResponseModel<string>>> UpdateBook(int bookId, [FromBody] Book book)
        {
            _logger.LogInformation($"Updating book with ID {bookId}");
            var response = new ResponseModel<string>();

            try
            {
                book.BookId = bookId;
                await _bookBusinessLayer.UpdateBookAsync(book);
                response.Success = true;
                response.Message = "Book updated successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book");
                response.Success = false;
                response.Message = "Failed to update book";
                return BadRequest(response);
            }
        }

        [HttpDelete("delete/{bookId}")]
        public async Task<ActionResult<ResponseModel<string>>> DeleteBook(int bookId)
        {
            _logger.LogInformation($"Deleting book with ID {bookId}");
            var response = new ResponseModel<string>();

            try
            {
                await _bookBusinessLayer.DeleteBookAsync(bookId);
                response.Success = true;
                response.Message = "Book deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book");
                response.Success = false;
                response.Message = "Failed to delete book";
                return BadRequest(response);
            }
        }
    }
}
