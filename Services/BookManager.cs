using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class BookManager : IBookService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerService _loggerService;

        public BookManager(IRepositoryManager repositoryManager, ILoggerService loggerService)
        {
            _repositoryManager = repositoryManager;
            _loggerService = loggerService;
        }

        public Book CreateOneBook(Book book)
        {
            _repositoryManager.Book.CreateOneBook(book);
            _repositoryManager.Save();
            return book;
        }

        public void DeleteOneBook(int id, bool trackChanges)
        {
            var entity = _repositoryManager.Book.GetOneBookById(id, trackChanges);
            if (entity is null)
            {
                string message = $"The book with id:{id} could not found!";
                _loggerService.LogInfo(message);
                throw new Exception(message);
            }

            _repositoryManager.Book.DeleteOneBook(entity);
            _repositoryManager.Save();
        }

        public IEnumerable<Book> GetAllBooks(bool trackChanges)
        {
            return _repositoryManager.Book.GetAllBooks(trackChanges);
        }

        public Book GetOneBookById(int id, bool trackChanges)
        {
            return _repositoryManager.Book.GetOneBookById(id, trackChanges);
        }

        public void UpdateOneBook(int id, Book book, bool trackChanges)
        {
            // check entity
            var entity = _repositoryManager.Book.GetOneBookById(id, trackChanges);
            if (entity is null)
            {
                string message = $"The book with id:{id} could not found!";
                _loggerService.LogInfo(message);
                throw new Exception(message);
            }

            // check params
            if (book is null)
                throw new ArgumentNullException(nameof(book));

            entity.Title = book.Title;
            entity.Price = book.Price;

            _repositoryManager.Book.Update(entity);
            _repositoryManager.Save();
        }
    }
}
