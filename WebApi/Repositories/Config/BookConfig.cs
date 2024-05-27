using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Models;

namespace WebApi.Repositories.Config
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasData(
                new Book {Id = 1, Title = "Satranç", Price = 60},
                new Book {Id = 2, Title = "Hayvan Çiftliği", Price = 150},
                new Book {Id = 3, Title = "Dorian Gray'in Portresi", Price = 130}
                );
        }
    }
}
