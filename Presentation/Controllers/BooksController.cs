using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public BooksController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _serviceManager.BookService.GetAllBooks(false);
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
                var book = _serviceManager
                    .BookService
                    .GetOneBookById(id, false);

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

                _serviceManager.BookService.CreateOneBook(book);

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
                if (book is null)
                    return BadRequest(); // 400

                _serviceManager.BookService.UpdateOneBook(id, book, true);
                return NoContent(); // 204
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
                _serviceManager.BookService.DeleteOneBook(id, false);
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
                var entity = _serviceManager
                    .BookService
                    .GetOneBookById(id, true);

                if (entity is null)
                    return NotFound(); // 404

                bookPatch.ApplyTo(entity);
                _serviceManager.BookService.UpdateOneBook(id, entity, true);

                return NoContent(); // 204
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

}
