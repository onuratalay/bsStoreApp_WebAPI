using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}
