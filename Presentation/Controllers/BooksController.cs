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
using Presentation.ActionFilters;
using Services.Contracts;

namespace Presentation.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
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
        public async Task<IActionResult> GetAllBooksAsync()
        {
            var books = await _serviceManager.BookService.GetAllBooksAsync(false);
            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync(int id)
        {
            var book = await _serviceManager
                .BookService
                .GetOneBookByIdAsync(id, false);

            return Ok(book);
        }

        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsert bookDto)
        {
            var book = await _serviceManager.BookService.CreateOneBookAsync(bookDto);

            return StatusCode(201, book);
            // StatusCode201 yerine CreatedAtRoute() kullanırsak
            // responseun headerına lokasyon bilgisi koyabiliyoruz.
            // Oluşan yeni kaynağın lokasyonunu yani URI'ını elde edebiliyoruz.
        }

        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] BookDtoForUpdate bookDto)
        {
            await _serviceManager.BookService.UpdateOneBookAsync(id, bookDto, false);
            return NoContent(); // 204
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {
            await _serviceManager.BookService.DeleteOneBookAsync(id, false);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest();

            var result = await _serviceManager.BookService.GetOneBookForPatchAsync(id, false);

            bookPatch.ApplyTo(result.bookDtoForUpdate, ModelState);

            TryValidateModel(result.bookDtoForUpdate);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            await _serviceManager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);

            return NoContent(); // 204
        }
    }
}