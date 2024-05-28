using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IBookService> _bookServiceLazy;
        public ServiceManager(IRepositoryManager repositoryManager)
        {
            _bookServiceLazy = new Lazy<IBookService>(() => new BookManager(repositoryManager));
        }

        public IBookService BookService => _bookServiceLazy.Value;
    }
}
