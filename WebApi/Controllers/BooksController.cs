using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public BooksController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _dbContext.Books.ToList();
                return Ok(books);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        [HttpGet("{id:int}")]
        public IActionResult GetOneBook(int id)
        {
            try
            {
                var book = _dbContext
                    .Books
                    .Where(b => b.Id.Equals(id))
                    .SingleOrDefault();

                if (book is null)
                    return NotFound();

                return Ok(book);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            try
            {
                if (book is null)
                    return BadRequest();

                _dbContext.Books.Add(book);
                _dbContext.SaveChanges();
                return StatusCode(201, book);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id,
            [FromBody] Book book)
        {
            try
            {
                // check book?
                var entity = _dbContext
                    .Books
                    .Where(b => b.Id.Equals(id))
                    .SingleOrDefault();

                if (entity is null)
                    return NotFound(); // 404

                // check id
                if (id != book.Id)
                    return BadRequest(); // 400

                entity.Title = book.Title;
                entity.Price = book.Price;
                _dbContext.SaveChanges();

                return Ok(book);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            try
            {
                var entity = _dbContext
                    .Books
                    .Where(b => b.Id.Equals(id))
                    .SingleOrDefault();

                if (entity is null)
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = $"Book with id:{id} could not found."
                    }); // 404

                _dbContext.Books.Remove(entity);
                _dbContext.SaveChanges();
                return NoContent();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id,
            [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            try
            {
                // check entity
                var entity = _dbContext
                    .Books
                    .Where(b => b.Id.Equals(id))
                    .SingleOrDefault();

                if (entity is null)
                    return NotFound(); // 404

                bookPatch.ApplyTo(entity);
                _dbContext.SaveChanges();

                return NoContent(); // 204
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
