using System.Collections;
using System.Text;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace WebApi.Utilities.Formatter
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type? type)
        {
            if (typeof(BookDto).IsAssignableFrom(type) ||
                typeof(IEnumerable<BookDto>).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }

            return false;

            // Bütün endpointlerde bu formatterı kullanmak istemeyebiliriz.
            // Bunun için bu methodu override edip sadece BookDto veya liste halinde
            // BookDto nesnesi dönen endpointler için kullanılabilir yapıyoruz.
        }

        private static void FormatCsv(StringBuilder buffer, BookDto book)
        {
            buffer.AppendLine($"{book.Id}, {book.Title}, {book.Price}");
            // AppendLine methodu StringBuilder ile gelir.
            // İnterpolasyon içindeki değişkenleri buffer nesnesine ekler ve alt satıra geçer. (\n)
            // $"{değişken}"  -string interpolation-
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();

            if (context.Object is IEnumerable<BookDto>)
            {
                foreach (var book in (IEnumerable<BookDto>) context.Object)
                {
                    FormatCsv(buffer, book);
                }
                {
                    
                }
            }
            else
            {
                    FormatCsv(buffer, (BookDto)context.Object);
            }

            await response.WriteAsync(buffer.ToString());
        }
    }
}

// Özellikle AI projelerinde CSV content negotiation
// desteğinin eklenmesi çok önemlidir.
