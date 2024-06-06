using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entities.DTOs;
using Entities.Exceptions;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class BookManager : IBookService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;

        public BookManager(IRepositoryManager repositoryManager, ILoggerService loggerService, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _loggerService = loggerService;
            _mapper = mapper;
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
                throw new BookNotFoundException(id);

            _repositoryManager.Book.DeleteOneBook(entity);
            _repositoryManager.Save();
        }

        public IEnumerable<Book> GetAllBooks(bool trackChanges)
        {
            return _repositoryManager.Book.GetAllBooks(trackChanges);
        }

        public Book GetOneBookById(int id, bool trackChanges)
        {
            var book = _repositoryManager.Book.GetOneBookById(id, trackChanges);

            if (book is null)
                throw new BookNotFoundException(id);
            return book;
        }

        public void UpdateOneBook(int id, BookDtoForUpdate bookDto, bool trackChanges)
        {
            // check entity
            var entity = _repositoryManager.Book.GetOneBookById(id, trackChanges);
            if (entity is null)
                throw new BookNotFoundException(id);

            // mapping
            entity = _mapper.Map<Book>(bookDto);

            _repositoryManager.Book.Update(entity);
            _repositoryManager.Save();
        }
    }
}
