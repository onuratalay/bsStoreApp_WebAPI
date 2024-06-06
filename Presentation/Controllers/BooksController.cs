using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DTOs;
using Entities.Exceptions;
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
            var books = _serviceManager.BookService.GetAllBooks(false);
            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetOneBook(int id)
        {
            var book = _serviceManager
                .BookService
                .GetOneBookById(id, false);

            return Ok(book);
        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            if (book is null)
                return BadRequest();

            _serviceManager.BookService.CreateOneBook(book);

            return StatusCode(201, book);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id,
            [FromBody] BookDtoForUpdate bookDto)
        {
            if (bookDto is null)
                return BadRequest(); // 400

            _serviceManager.BookService.UpdateOneBook(id, bookDto, true);
            return NoContent(); // 204
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            _serviceManager.BookService.DeleteOneBook(id, false);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id,
            [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            // check entity
            var entity = _serviceManager
                .BookService
                .GetOneBookById(id, true);

            bookPatch.ApplyTo(entity);
            _serviceManager.BookService.UpdateOneBook(id,
                new BookDtoForUpdate(entity.Id, entity.Title, entity.Price),
                true);

            return NoContent(); // 204
        }
    }

}
